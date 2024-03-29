﻿using AutoMapper;
using Business.Hub;
using Business.Utils;
using Business.Utils.Log;
using Entity.Common;
using FreeSql;
using Microservice.Library.Container;
using Microservice.Library.DataMapping.Gen;
using Microservice.Library.Extension;
using Microservice.Library.Extension.Helper;
using Microservice.Library.File;
using Microservice.Library.FreeSql.Gen;
using Microsoft.AspNetCore.SignalR;
using Model.Common;
using Model.Common.ChunkFileMergeTaskDTO;
using Model.Utils.Config;
using Model.Utils.Log;
using Model.Utils.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handler
{
    /// <summary>
    /// 分片文件合并处理类
    /// </summary>
    public class ChunkFileMergeHandler
    {
        #region 私有成员

        /// <summary>
        /// 数据队列
        /// </summary>
        /// <remarks>
        /// </remarks>
        readonly ConcurrentQueue<string> IdQueue = new();

        /// <summary>
        /// 发送分片来源信息任务取消令牌集合
        /// </summary>
        /// <remarks>{Key, CancellationTokenSource}</remarks>
        public static readonly ConcurrentDictionary<string, CancellationTokenSource> CancellationTokens = new();

        SystemConfig Config
        {
            get
            {
                if (_Config == null)
                    _Config = AutofacHelper.GetService<SystemConfig>();
                return _Config;
            }
        }

        SystemConfig _Config;

        IFreeSql Orm
        {
            get
            {
                if (_Orm == null)
                    _Orm = AutofacHelper.GetService<IFreeSqlProvider>().GetFreeSql();
                return _Orm;
            }
        }

        IFreeSql _Orm;

        IBaseRepository<Common_File, string> Repository_File
        {
            get
            {
                if (_Repository_File == null)
                    _Repository_File = Orm.GetRepository<Common_File, string>();
                return _Repository_File;
            }
        }

        IBaseRepository<Common_File, string> _Repository_File;

        IBaseRepository<Common_ChunkFile, string> Repository_FileChunk
        {
            get
            {
                if (_Repository_FileChunk == null)
                    _Repository_FileChunk = Orm.GetRepository<Common_ChunkFile, string>();
                return _Repository_FileChunk;
            }
        }

        IBaseRepository<Common_ChunkFile, string> _Repository_FileChunk;

        IBaseRepository<Common_ChunkFileMergeTask, string> Repository_ChunkFileMergeTask
        {
            get
            {
                if (_Repository_ChunkFileMergeTask == null)
                    _Repository_ChunkFileMergeTask = Orm.GetRepository<Common_ChunkFileMergeTask, string>();
                return _Repository_ChunkFileMergeTask;
            }
        }

        IBaseRepository<Common_ChunkFileMergeTask, string> _Repository_ChunkFileMergeTask;

        /// <summary>
        /// 存储路径根目录相对路径
        /// </summary>
        string BaseDir => $"{Config.AbsoluteStorageDirectory}/upload/{DateTime.Now:yyyy-MM-dd}";

        IHubContext<ChunkFileMergeTaskHub> CFMTHub
        {
            get
            {
                if (_CFMTHub == null)
                    _CFMTHub = AutofacHelper.GetService<IHubContext<ChunkFileMergeTaskHub>>();
                return _CFMTHub;
            }
        }

        IHubContext<ChunkFileMergeTaskHub> _CFMTHub;

        //IMapper Mapper
        //{
        //    get
        //    {
        //        if (_Mapper == null)
        //            _Mapper = AutofacHelper.GetService<IAutoMapperProvider>().GetMapper();
        //        return _Mapper;
        //    }
        //}

        //IMapper _Mapper;

        TaskCompletionSource<bool> TCS;

        /// <summary>
        /// 获取任务
        /// </summary>
        /// <param name="id">任务Id</param>
        /// <param name="task">任务</param>
        /// <returns></returns>
        bool GetTask(string id, out Common_ChunkFileMergeTask task)
        {
            task = Repository_ChunkFileMergeTask.Find(id);

            if (task == null)
            {
                Logger.Log(
                    NLog.LogLevel.Warn,
                    LogType.警告信息,
                    $"合并分片文件失败, 指定的任务不存在[ID: {id}].");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 是否文件早已存在
        /// </summary>
        /// <param name="task">任务</param>
        /// <returns></returns>
        bool IsFileAlready(Common_ChunkFileMergeTask task)
        {
            var alreadyPath = Repository_File.Where(o => o.MD5 == task.MD5
                                                 && o.ServerKey == Config.ServerKey
                                                 && o.State == $"{FileState.可用}")
                                        .ToOne(o => o.Path);

            if (alreadyPath != default
                && Repository_File.Where(o => o.MD5 == task.MD5 && o.ServerKey == Config.ServerKey && o.State != $"{FileState.可用}")
                                .Any())
            {
                if (Repository_File.UpdateDiy.Where(o => o.MD5 == task.MD5
                             && o.ServerKey == Config.ServerKey
                             && o.State != $"{FileState.可用}")
                        .Set(o => o.State, FileState.可用)
                        .Set(o => o.Path, alreadyPath)
                        .ExecuteAffrows() < 0)
                {
                    task.State = CFMTState.失败;
                    task.Info = "合并分片文件失败, 更新文件信息失败.";
                    task.ModifyTime = DateTime.Now;
                    Repository_ChunkFileMergeTask.Update(task);

                    _ = SendUpdateData(
                        task.Id,
                        new
                        {
                            task.State,
                            task.Info,
                            ModifyTime = task.ModifyTime?.ToString("yyyy-MM-dd HH:mm:ss.ffff")
                        });

                    Logger.Log(
                        NLog.LogLevel.Warn,
                        LogType.警告信息,
                        $"合并分片文件失败, 更新文件信息失败[ID: {task.Id}].");
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取分片文件
        /// </summary>
        /// <param name="task">任务</param>
        /// <param name="allChunkFiles">全部分片文件</param>
        /// <param name="needChunkFiles">合并所需的分片文件</param>
        /// <returns></returns>
        bool GetChunkFiles(Common_ChunkFileMergeTask task, out List<(string Id, string Path)> allChunkFiles, out List<(string Id, int Index, long Bytes, string Path)> needChunkFiles)
        {
            var where = LinqHelper.True<Common_ChunkFile>().AndAlso(o => o.FileMD5 == task.MD5 && o.ServerKey == task.ServerKey && o.Specs == task.Specs && o.State == $"{FileState.可用}");

            if (Repository_FileChunk.Where(where).GroupBy(o => o.Index).Count() != task.Total)
            {
                //IdQueue.Enqueue(task.Id);

                task.Info = "合并分片文件失败, 分片文件还未全部上传完毕.";
                //task.Info = "合并分片文件失败, 分片文件还未全部上传完毕, 已将任务移至队尾.";
                task.ModifyTime = DateTime.Now;
                Repository_ChunkFileMergeTask.Update(task);

                _ = SendUpdateData(
                    task.Id,
                    new
                    {
                        task.Info,
                        ModifyTime = task.ModifyTime?.ToString("yyyy-MM-dd HH:mm:ss.ffff")
                    });

                Logger.Log(
                    NLog.LogLevel.Warn,
                    LogType.警告信息,
                    $"合并分片文件失败, 分片文件还未全部上传完毕[ID: {task.Id}].");
                //$"合并分片文件失败, 分片文件还未全部上传完毕, 已将任务移至队尾[ID: {task.Id}].");

                allChunkFiles = null;
                needChunkFiles = null;
                return false;
            }

            var _allChunkFiles = Repository_FileChunk.Where(where).ToList(o => new { o.Id, o.Index, o.Bytes, o.Path });

            _allChunkFiles.RemoveAll(o =>
            {
                if (!FileHelper.Exists(o.Path))
                {
                    Repository_FileChunk.Delete(o.Id);
                    return true;
                }
                return false;
            });

            var _needChunkFiles = _allChunkFiles.GroupBy(o => o.Index)
                                         .OrderBy(o => o.Key);

            if (_needChunkFiles.Count() != task.Total)
            {
                task.State = CFMTState.失败;
                task.Info = "合并分片文件失败, 部分分片文件已损坏.";
                task.ModifyTime = DateTime.Now;
                Repository_ChunkFileMergeTask.Update(task);

                _ = SendUpdateData(
                    task.Id,
                    new
                    {
                        task.State,
                        task.Info,
                        ModifyTime = task.ModifyTime?.ToString("yyyy-MM-dd HH:mm:ss.ffff")
                    });

                Logger.Log(
                    NLog.LogLevel.Warn,
                    LogType.警告信息,
                    $"合并分片文件失败, 部分分片文件已损坏[ID: {task.Id}].");

                allChunkFiles = null;
                needChunkFiles = null;
                return false;
            }

            allChunkFiles = _allChunkFiles.Select(o => (o.Id, o.Path))
                                        .ToList();

            needChunkFiles = _needChunkFiles.Select(o => o.First())
                                            .Select(o => (o.Id, o.Index, o.Bytes.Value, o.Path))
                                            .ToList();

            //获取文件信息
            //var id = needChunkFiles.First().Id;
            //var fileInfo = Repository_FileChunk.Where(o => o.Id == id)
            //                                .ToOne(o => new { o.ContentType, o.Extension });
            //task.ContentType = fileInfo.ContentType;
            //task.Extension = fileInfo.Extension;
            //task.FullName = $"{task.Name}{task.Extension}";

            return true;
        }

        /// <summary>
        /// 合并分片文件
        /// </summary>
        /// <param name="task">任务</param>
        /// <param name="needChunkFiles">合并所需的分片文件</param>
        async Task Merge(Common_ChunkFileMergeTask task, List<(string Id, int Index, long Bytes, string Path)> needChunkFiles)
        {
            task.State = CFMTState.处理中;
            task.Info = "分片文件合并中.";
            task.ModifyTime = DateTime.Now;
            Repository_ChunkFileMergeTask.Update(task);

            _ = SendUpdateData(
                task.Id,
                new
                {
                    task.State,
                    task.Info,
                    ModifyTime = task.ModifyTime?.ToString("yyyy-MM-dd HH:mm:ss.ffff")
                });

            if (!Directory.Exists(BaseDir))
                Directory.CreateDirectory(BaseDir);

            var path = Path.Combine(BaseDir, $"{Guid.NewGuid()}{task.Extension}");

            using var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            foreach (var chunkFile in needChunkFiles)
            {
                if (task.CurrentChunkIndex >= chunkFile.Index)
                {
                    fs.Seek(chunkFile.Bytes, SeekOrigin.Current);
                    continue;
                }

                using var cfs = new FileStream(chunkFile.Path, FileMode.Open, FileAccess.Read);
                await cfs.CopyToAsync(fs);

                task.CurrentChunkIndex = chunkFile.Index;
                task.ModifyTime = DateTime.Now;
                task.Bytes = (task.Bytes ?? 0) + chunkFile.Bytes;
                Repository_ChunkFileMergeTask.Update(task);

                _ = SendUpdateData(
                    task.Id,
                    new
                    {
                        task.CurrentChunkIndex,
                        task.Bytes,
                        ModifyTime = task.ModifyTime?.ToString("yyyy-MM-dd HH:mm:ss.ffff")
                    });
            }

            task.Size = FileHelper.GetFileSize(task.Bytes ?? 0);
            task.Path = path;
        }

        /// <summary>
        /// 已完成
        /// </summary>
        /// <param name="task">任务</param>
        /// <returns></returns>
        bool Completed(Common_ChunkFileMergeTask task)
        {
            task.State = CFMTState.已完成;
            task.ModifyTime = DateTime.Now;
            task.CompletedTime = DateTime.Now;

            if (Repository_File.UpdateDiy.Where(o => o.MD5 == task.MD5
                         && o.ServerKey == Config.ServerKey
                         && o.State != $"{FileState.可用}")
                    .Set(o => o.State, FileState.可用)
                    .Set(o => o.ContentType, task.ContentType)
                    .Set(o => o.Extension, task.Extension)
                    .Set(o => o.Name, task.Name)
                    .Set(o => o.FileType, FileType.GetFileTypeByExtension(task.Extension))
                    .Set(o => o.Bytes, task.Bytes)
                    .Set(o => o.Size, task.Size)
                    .Set(o => o.Path, task.Path)
                    .ExecuteAffrows() < 0)
            {
                task.Info = "合并分片文件成功, 但更新文件信息失败.";
                Repository_ChunkFileMergeTask.Update(task);

                _ = SendUpdateData(
                    task.Id,
                    new
                    {
                        task.State,
                        task.Info,
                        ModifyTime = task.ModifyTime?.ToString("yyyy-MM-dd HH:mm:ss.ffff"),
                        CompletedTime = task.CompletedTime?.ToString("yyyy-MM-dd HH:mm:ss.ffff")
                    });

                Logger.Log(
                    NLog.LogLevel.Warn,
                    LogType.警告信息,
                    $"合并分片文件失败, 更新文件信息失败[ID: {task.Id}].");

                return false;
            }

            task.Info = "已合并所有分片文件.";
            Repository_ChunkFileMergeTask.Update(task);

            //CancelSendChunksSourceInfoTask(task.MD5, task.Specs, task.Total);

            _ = SendUpdateData(
                task.Id,
                new
                {
                    task.State,
                    task.Info,
                    ModifyTime = task.ModifyTime?.ToString("yyyy-MM-dd HH:mm:ss.ffff"),
                    CompletedTime = task.CompletedTime?.ToString("yyyy-MM-dd HH:mm:ss.ffff")
                });

            return true;
        }

        /// <summary>
        /// 清理
        /// </summary>
        /// <param name="chunkFiles">分片文件</param>
        void Clear(List<(string Id, string Path)> chunkFiles)
        {
            chunkFiles.ForEach(o => File.Delete(o.Path));

            var ids = chunkFiles.Select(o => o.Id).ToList();
            Repository_FileChunk.UpdateDiy.Where(o => ids.Contains(o.Id))
                                        .Set(o => o.State, FileState.已删除)
                                        .ExecuteAffrows();
        }

        /// <summary>
        /// 运行
        /// </summary>
        async void Run()
        {
            while (true)
            {
                if (TCS != null)
                {
                    if (!await TCS.Task)
                        return;
                    TCS = null;
                }

                if (IdQueue.IsEmpty)
                {
                    TCS = new TaskCompletionSource<bool>();
                    continue;
                }

                try
                {
                    await Processing();
                }
                catch (Exception ex)
                {
                    Logger.Log(
                        NLog.LogLevel.Error,
                        LogType.系统异常,
                        "合并分片文件时异常.",
                        null,
                        ex);
                }
            }
        }

        /// <summary>
        /// 处理
        /// </summary>
        async Task Processing()
        {
            var Queue = IdQueue;
            var Count = Queue.Count;

            for (int i = 0; i < Count; i++)
            {
                Queue.TryDequeue(out string id);

                try
                {
                    if (!GetTask(id, out Common_ChunkFileMergeTask task))
                        continue;

                    if (IsFileAlready(task))
                        continue;

                    if (!GetChunkFiles(task, out List<(string Id, string Path)> allChunkFiles, out List<(string Id, int Index, long Bytes, string Path)> needChunkFiles))
                        continue;

                    await Merge(task, needChunkFiles);

                    if (!Completed(task))
                        continue;

                    CancelSendChunksSourceInfoTask(task.MD5, task.Specs, task.Total);
                    await SendChunksSourceInfo(task.MD5, task.Specs, task.Total);

                    Clear(allChunkFiles);
                }
                catch (Exception ex)
                {
                    Logger.Log(
                        NLog.LogLevel.Error,
                        LogType.系统异常,
                        $"合并分片文件时异常.",
                        null,
                        ex);
                }
            }
        }

        /// <summary>
        /// 发送更新数据
        /// </summary>
        /// <param name="id">任务ID</param>
        /// <param name="data">更新的数据</param>
        /// <returns></returns>
        async Task SendUpdateData(string id, object data)
        {
            await CFMTHub.Clients.All.SendCoreAsync(
                 CFMTHubMethod.更新任务,
                 new object[]
                 {
                    id,
                    data
                 });
        }

        /// <summary>
        /// 获取分片热度信息
        /// </summary>
        /// <param name="total"></param>
        /// <param name="chunkFileIndices"></param>
        /// <returns></returns>
        static List<ActivityInfo> GetActivityInfo(int total, Dictionary<int, int> chunkFileIndices)
        {
            var activitys = new List<ActivityInfo>();
            var lastActivity = 0;
            for (int i = 0; i < total; i++)
            {
                var activity = chunkFileIndices.ContainsKey(i) ? chunkFileIndices[i] : 0;

                if (lastActivity != activity)
                {
                    activitys.Add(new ActivityInfo
                    {
                        Activity = activity,
                        Percentage = 1
                    });

                    lastActivity = activity;
                }
                else
                {
                    if (activitys.Count == 0)
                        activitys.Add(new ActivityInfo
                        {
                            Activity = activity,
                            Percentage = 1
                        });
                    else
                        activitys.Last().Percentage += 1;
                }
            }

            activitys.ForEach(o => o.Percentage = Math.Round(o.Percentage / total, 4) * 100);

            return activitys;
        }

        /// <summary>
        /// 获取分片文件来源信息
        /// </summary>
        /// <param name="md5"></param>
        /// <param name="specs"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        async Task SendChunksSourceInfo(string md5, int specs, int total)
        {
            var chunksSource = new ChunksSourceInfo
            {
                Specs = specs,
                Total = total
            };

            var chunkFileIndices = Repository_FileChunk.Where(p =>
                            p.FileMD5 == md5
                            && p.Specs == specs
                            && (p.State == $"{FileState.上传中}" || p.State == $"{FileState.可用}")
                            && p.ServerKey == Config.ServerKey)
                       .GroupBy(p => p.Index)
                       .OrderBy(p => p.Key)
                       .ToDictionary(p => p.Count());

            chunksSource.Activitys = GetActivityInfo(total, chunkFileIndices);

            await CFMTHub.Clients.All.SendCoreAsync(
                   CFMTHubMethod.更新分片来源信息,
                   new object[]
                   {
                        md5,
                        chunksSource
                   });
        }

        /// <summary>
        /// 发送分片来源信息任务
        /// </summary>
        /// <param name="md5">文件MD5值</param>
        /// <param name="specs">分片规格</param>
        /// <param name="total">分片总数</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task SendChunksSourceInfoTask(string md5, int specs, int total, CancellationToken cancellationToken)
        {
            void action(object state)
            {
                if (state == null)
                    return;

                var paramsType = typeof((string md5, int specs, int total));

                var _md5 = (string)paramsType.GetField("Item1").GetValue(state);
                var _specs = (int)paramsType.GetField("Item2").GetValue(state);
                var _total = (int)paramsType.GetField("Item3").GetValue(state);

                var lastChunkFileUpload = DateTime.MinValue;
                var lastTaskCurrentChunkIndex = -2;

                while (Repository_ChunkFileMergeTask.Where(o => o.MD5 == _md5 && o.Specs == _specs && o.State == $"{CFMTState.上传中}").Any())
                {
                    var _lastChunkFileUpload = Repository_FileChunk.Where(o => o.FileMD5 == md5 && o.Specs == specs && o.State != $"{FileState.已删除}")
                                                        .OrderByDescending(o => o.CreateTime)
                                                        .ToOne(o => o.CreateTime);

                    if (lastChunkFileUpload == _lastChunkFileUpload)
                    {
                        var _lastTaskCurrentChunkIndex = Repository_ChunkFileMergeTask.Where(o => o.MD5 == md5 && o.Specs == specs)
                                                            .OrderByDescending(o => o.ModifyTime)
                                                            .ToOne(o => o.CurrentChunkIndex);

                        if (lastTaskCurrentChunkIndex == _lastTaskCurrentChunkIndex)
                        {
                            Task.Delay(500, cancellationToken).GetAwaiter().GetResult();
                            continue;
                        }
                        else
                            lastTaskCurrentChunkIndex = _lastTaskCurrentChunkIndex;
                    }
                    else
                        lastChunkFileUpload = _lastChunkFileUpload;

                    SendChunksSourceInfo(_md5, _specs, _total).GetAwaiter().GetResult();

                    Task.Delay(500, cancellationToken).GetAwaiter().GetResult();
                }
            }

            return new Task(action, (md5, specs, total), cancellationToken);
        }

        /// <summary>
        /// 开始发送分片信息任务
        /// </summary>
        /// <param name="md5">文件MD5值</param>
        /// <param name="specs">分片规格</param>
        /// <param name="total">分片总数</param>
        /// <returns></returns>
        void BeginSendChunksSourceInfoTask(string md5, int specs, int total)
        {
            var timeoutCancel = new CancellationTokenSource();

            timeoutCancel.Token.Register(state =>
            {

            }, (md5, specs, total));

            SendChunksSourceInfoTask(md5, specs, total, timeoutCancel.Token).Start();

            CancellationTokens.AddOrUpdate(
                $"{md5}{specs}{total}",
                timeoutCancel,
                (key, old) =>
                {
                    return timeoutCancel;
                });
        }

        /// <summary>
        /// 取消发送分片信息任务
        /// </summary>
        /// <param name="md5">文件MD5值</param>
        /// <param name="specs">分片规格</param>
        /// <param name="total">分片总数</param>
        static void CancelSendChunksSourceInfoTask(string md5, int specs, int total)
        {
            if (CancellationTokens.TryRemove($"{md5}{specs}{total}", out CancellationTokenSource cts) && cts.Token.CanBeCanceled)
                cts.Cancel();
        }

        #endregion

        #region 公共部分

        /// <summary>
        /// 名称
        /// </summary>
        public const string Name = "分片文件合并模块";

        /// <summary>
        /// 启动时间
        /// </summary>
        public DateTime? StartTime;

        /// <summary>
        /// 数据总量
        /// </summary>
        public int DataCount => IdQueue.Count;

        /// <summary>
        /// 当前状态
        /// </summary>
        /// <returns></returns>
        public bool? State()
        {
            return TCS?.Task.Status == TaskStatus.RanToCompletion ? TCS?.Task.Result : null;
        }

        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            TCS = new TaskCompletionSource<bool>();

            StartTime = DateTime.Now;

            //将未完成的任务添加至队列
            Repository_ChunkFileMergeTask.Where(o => o.State != $"{CFMTState.已完成}" && o.ServerKey == Config.ServerKey)
                .ToList(o => o.Id)
                .ForEach(o => IdQueue.Enqueue(o));

            Run();

            //开始合并
            TCS?.SetResult(true);
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Shutdown()
        {
            if (TCS == null)
                TCS = new TaskCompletionSource<bool>();

            StartTime = DateTime.Now;

            TCS?.SetResult(false);

            IdQueue.Clear();
        }

        /// <summary>
        /// 新增合并任务
        /// </summary>
        /// <param name="md5">文件MD5值</param>
        /// <param name="type">文件类型</param>
        /// <param name="extension">文件拓展名</param>
        /// <param name="name">文件名(不包括拓展名)</param>
        /// <param name="specs">分片文件规格</param>
        /// <param name="total">分片文件总数</param>
        public void Add(string md5, string type, string extension, string name, int specs, int total)
        {
            if (Repository_ChunkFileMergeTask.Where(o => o.MD5 == md5 && o.Specs == specs && o.Total == total && o.ServerKey == Config.ServerKey).Any())
                return;

            var newTask = new Common_ChunkFileMergeTask
            {
                ServerKey = Config.ServerKey,
                MD5 = md5,
                ContentType = type,
                Extension = extension,
                Name = name,
                Specs = specs,
                Total = total,
                State = CFMTState.上传中,
                CurrentChunkIndex = -1
            }.InitEntityWithoutOP();

            Repository_ChunkFileMergeTask.Insert(newTask);

            CFMTHub.Clients.All.SendCoreAsync(
                CFMTHubMethod.新增任务,
                new object[]
                {
                    new
                    {
                        newTask.Id,
                        newTask.ServerKey,
                        newTask.MD5,
                        newTask.Name,
                        newTask.ContentType,
                        newTask.Extension,
                        newTask.Bytes,
                        newTask.Size,
                        newTask.Path,
                        newTask.Specs,
                        newTask.Total,
                        newTask.State,
                        newTask.Info,
                        CreateTime = newTask.CreateTime.ToString("yyyy-MM-dd HH:mm:ss.ffff"),
                        ModifyTime = newTask.ModifyTime?.ToString("yyyy-MM-dd HH:mm:ss.ffff"),
                        CompletedTime = newTask.CompletedTime?.ToString("yyyy-MM-dd HH:mm:ss.ffff"),
                        FullName = $"{newTask.Name}{newTask.Extension}",
                    }
                });

            BeginSendChunksSourceInfoTask(newTask.MD5, newTask.Specs, newTask.Total);
        }

        /// <summary>
        /// 处理合并任务
        /// </summary>
        /// <param name="md5">文件MD5值</param>
        /// <param name="specs">分片文件规格</param>
        /// <param name="total">分片文件总数</param>
        public void Handler(string md5, int specs, int total)
        {
            var taskId = Repository_ChunkFileMergeTask.Where(o => o.MD5 == md5 && o.Specs == specs && o.Total == total && o.ServerKey == Config.ServerKey)
                 .ToOne(o => o.Id);

            _ = Repository_ChunkFileMergeTask.UpdateDiy
                 .Where(o => o.Id == taskId).Set(o => o.State, $"{CFMTState.等待处理}")
                 .ExecuteAffrows();

            _ = SendUpdateData(
                taskId,
                new
                {
                    State = CFMTState.等待处理
                });

            IdQueue.Enqueue(taskId);

            //开始合并
            TCS?.SetResult(true);
        }

        #endregion
    }
}

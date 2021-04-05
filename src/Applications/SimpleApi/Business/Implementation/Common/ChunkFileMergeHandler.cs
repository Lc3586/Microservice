using Business.Utils;
using Business.Utils.Log;
using Entity.Common;
using FreeSql;
using Microservice.Library.Container;
using Microservice.Library.Extension;
using Microservice.Library.File;
using Microservice.Library.FreeSql.Gen;
using Microservice.Library.Http;
using Model.Common;
using Model.Utils.Config;
using Model.Utils.Log;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Implementation.Common
{
    /// <summary>
    /// 合并分片文件
    /// </summary>
    public class ChunkFileMergeHandler
    {
        #region 私有成员

        /// <summary>
        /// 数据队列
        /// </summary>
        /// <remarks>
        /// </remarks>
        readonly ConcurrentQueue<string> IdQueue = new ConcurrentQueue<string>();

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
        static string BaseDir => Path.GetDirectoryName($"\\upload\\{DateTime.Now:yyyy-MM-dd}\\");

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
            var select = Repository_FileChunk.Where(o => o.FileMD5 == task.MD5 && o.ServerKey == task.ServerKey && o.Specs == task.Specs && o.State == $"{FileState.可用}");

            if (select.GroupBy(o => o.Index).Count() != task.Total)
            {
                IdQueue.Enqueue(task.Id);

                task.Info = "合并分片文件失败, 分片文件还未全部上传完毕, 已将任务移至队尾.";
                task.ModifyTime = DateTime.Now;
                Repository_ChunkFileMergeTask.Update(task);

                Logger.Log(
                    NLog.LogLevel.Warn,
                    LogType.警告信息,
                    $"合并分片文件失败, 分片文件还未全部上传完毕, 已将任务移至队尾[ID: {task.Id}].");

                allChunkFiles = null;
                needChunkFiles = null;
                return false;
            }

            var _allChunkFiles = select.ToList(o => new { o.Id, o.Index, o.Bytes, o.Path });

            _allChunkFiles.RemoveAll(o =>
            {
                if (!FileHelper.Exists(o.Path))
                {
                    Repository_FileChunk.Delete(o.Id);
                    return true;
                }
                return false;
            });

            needChunkFiles = _allChunkFiles.GroupBy(o => o.Index)
                                        .OrderBy(o => o.Key)
                                        .Select(o => o.First())
                                        .Select(o => (o.Id, o.Index, o.Bytes.Value, o.Path))
                                        .ToList();

            if (needChunkFiles.Count != task.Total)
            {
                task.State = CFMTState.失败;
                task.Info = "合并分片文件失败, 部分分片文件已损坏.";
                task.ModifyTime = DateTime.Now;
                Repository_ChunkFileMergeTask.Update(task);

                Logger.Log(
                    NLog.LogLevel.Warn,
                    LogType.警告信息,
                    $"合并分片文件失败, 部分分片文件已损坏[ID: {task.Id}].");

                allChunkFiles = null;
                return false;
            }

            allChunkFiles = _allChunkFiles.Select(o => (o.Id, o.Path))
                                        .ToList();

            //获取文件信息
            var id = needChunkFiles.First().Id;
            var fileInfo = Repository_FileChunk.Where(o => o.Id == id).ToOne(o => new { o.ContentType, o.Extension });
            task.ContentType = fileInfo.ContentType;
            task.Extension = fileInfo.Extension;
            task.FullName = $"{task.Name}{task.Extension}";

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

            var baseDirPath = PathHelper.GetAbsolutePath($"~{BaseDir}");

            if (!Directory.Exists(baseDirPath))
                Directory.CreateDirectory(baseDirPath);

            var path = Path.Combine(baseDirPath, $"{Guid.NewGuid()}{task.Extension}");

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
                    .Set(o => o.FileType, FileType.GetFileType(task.Extension))
                    .Set(o => o.FullName, task.FullName)
                    .Set(o => o.Bytes, task.Bytes)
                    .Set(o => o.Size, task.Size)
                    .Set(o => o.Path, task.Path)
                    .ExecuteAffrows() < 0)
            {
                task.Info = "合并分片文件失败, 更新文件信息失败.";
                Repository_ChunkFileMergeTask.Update(task);

                Logger.Log(
                    NLog.LogLevel.Warn,
                    LogType.警告信息,
                    $"合并分片文件失败, 更新文件信息失败[ID: {task.Id}].");

                return false;
            }

            task.Info = "已合并所有分片文件.";
            Repository_ChunkFileMergeTask.Update(task);

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

        #endregion

        #region 公共部分

        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            //将未完成的任务添加至队列
            Repository_ChunkFileMergeTask.Where(o => o.State != $"{CFMTState.已完成}" && o.ServerKey == Config.ServerKey)
                .ToList(o => o.Id)
                .ForEach(o => IdQueue.Enqueue(o));

            Run();
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Shutdown()
        {
            if (TCS == null)
                TCS = new TaskCompletionSource<bool>();

            TCS?.SetResult(false);

            IdQueue.Clear();
        }

        /// <summary>
        /// 新增合并任务
        /// </summary>
        /// <param name="md5">文件MD5值</param>
        /// <param name="name">文件名(不包括拓展名)</param>
        /// <param name="specs">分片文件规格</param>
        /// <param name="total">分片文件总数</param>
        public void Add(string md5, string name, int specs, int total)
        {
            var newTask = new Common_ChunkFileMergeTask
            {
                ServerKey = Config.ServerKey,
                MD5 = md5,
                Name = name,
                Specs = specs,
                Total = total,
                State = CFMTState.等待处理
            }.InitEntityWithoutOP();

            Repository_ChunkFileMergeTask.Insert(newTask);

            IdQueue.Enqueue(newTask.Id);

            //开始合并
            TCS?.SetResult(true);
        }

        /// <summary>
        /// 运行
        /// </summary>
        private async void Run()
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
        private async Task Processing()
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

        #endregion
    }
}

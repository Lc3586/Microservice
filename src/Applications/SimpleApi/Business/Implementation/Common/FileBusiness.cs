using AutoMapper;
using Business.Handler;
using Business.Interface.Common;
using Business.Utils;
using Business.Utils.Log;
using Business.Utils.Pagination;
using Entity.Common;
using FreeSql;
using ImageProcessorCore;
using Microservice.Library.Container;
using Microservice.Library.DataMapping.Gen;
using Microservice.Library.Extension;
using Microservice.Library.File;
using Microservice.Library.FreeSql.Extention;
using Microservice.Library.FreeSql.Gen;
using Microservice.Library.OpenApi.Extention;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Model.Common;
using Model.Common.FileDTO;
using Model.Utils.Log;
using Model.Utils.Pagination;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using FileInfo = Model.Common.FileDTO.FileInfo;

namespace Business.Implementation.Common
{
    /// <summary>
    /// 文件处理业务类
    /// </summary>
    public class FileBusiness : BaseBusiness, IFileBusiness, IDependency
    {
        #region DI

        public FileBusiness(
            IFreeSqlProvider freeSqlProvider,
            IAutoMapperProvider autoMapperProvider,
            IHttpContextAccessor httpContextAccessor,
            ChunkFileMergeHandler mergeHandler)
        {
            Orm = freeSqlProvider.GetFreeSql();
            Mapper = autoMapperProvider.GetMapper();
            Repository = Orm.GetRepository<Common_File, string>();
            Repository_FileChunk = Orm.GetRepository<Common_ChunkFile, string>();
            HttpContextAccessor = httpContextAccessor;
            MergeHandler = mergeHandler;
            FileStateDir = $"{Config.AbsoluteWWWRootDirectory}/filestate";
            PreviewDir = $"{Config.AbsoluteWWWRootDirectory}/filetypes";
            BaseDir = $"{Config.AbsoluteStorageDirectory}/upload/{DateTime.Now:yyyy-MM-dd}";
        }

        #endregion

        #region 私有成员

        readonly IMapper Mapper;

        readonly IFreeSql Orm;

        readonly IBaseRepository<Common_File, string> Repository;

        readonly IBaseRepository<Common_ChunkFile, string> Repository_FileChunk;

        readonly IHttpContextAccessor HttpContextAccessor;

        readonly ChunkFileMergeHandler MergeHandler;

        /// <summary>
        /// 文件状态图存储路径根目录绝对路径
        /// </summary>
        readonly string FileStateDir;

        /// <summary>
        /// 文件类型预览图存储路径根目录绝对路径
        /// </summary>
        readonly string PreviewDir;

        /// <summary>
        /// 存储路径根目录绝对路径
        /// </summary>
        readonly string BaseDir;

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="image">图像</param>
        /// <param name="path">绝对路径</param>
        static void Save(Image image, string path)
        {
            try
            {
                using var fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                image.Save(fs);
            }
            catch (Exception ex)
            {
                //var code = Marshal.GetLastWin32Error();
                //throw new MessageException($"文件不支持: {code}", ex);
                throw new MessageException("文件保存失败.", ex);
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="bytes">文件</param>
        /// <param name="path">绝对路径</param>
        static async Task Save(byte[] bytes, string path)
        {
            try
            {
                using var stream = File.Create(path);
                await stream.WriteAsync(bytes);
            }
            catch (Exception ex)
            {
                throw new MessageException("文件保存失败.", ex);
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="stream">流</param>
        /// <param name="path">绝对路径</param>
        static async Task Save(Stream stream, string path)
        {
            try
            {
                stream.Seek(0, SeekOrigin.Begin);
                using var fs = File.Create(path);
                await stream.CopyToAsync(fs);
            }
            catch (Exception ex)
            {
                var code = Marshal.GetLastWin32Error();
                throw new MessageException($"文件不支持: {code}", ex);
            }
        }

        /// <summary>
        /// 获取预览图
        /// </summary>
        /// <param name="extension">文件后缀</param>
        /// <returns></returns>
        string GetPreviewImage(string extension = null)
        {
            if (extension.IsNullOrEmpty())
                goto empty;

            var previewfile = Path.Combine(PreviewDir, $"{extension.TrimStart('.')}.png");
            if (File.Exists(previewfile))
                return previewfile;

            empty:
            return Path.Combine(PreviewDir, "empty.png");
        }

        /// <summary>
        /// 输出图片
        /// </summary>
        /// <param name="response"></param>
        /// <param name="img"></param>
        static async Task ResponseImage(HttpResponse response, Image img)
        {
            using MemoryStream ms = new MemoryStream();
            img.Save(ms);
            response.ContentLength = ms.Length;
            await ms.CopyToAsync(response.Body);
        }

        /// <summary>
        /// 输出图片
        /// </summary>
        /// <param name="response"></param>
        /// <param name="stream">流</param>
        static async Task ResponseImage(HttpResponse response, Stream stream)
        {
            response.ContentLength = stream.Length;
            await stream.CopyToAsync(response.Body);
        }

        /// <summary>
        /// 输出文件
        /// </summary>
        /// <param name="response"></param>
        /// <param name="path"></param>
        static async Task ResponseFile(HttpResponse response, string path)
        {
            if (!File.Exists(path))
                throw new MessageException("文件不存在或已被删除");

            await response.SendFileAsync(path);
            //using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            //response.ContentLength = fs.Length;
            //await fs.CopyToAsync(response.Body);
        }

        /// <summary>
        /// 输出错误
        /// </summary>
        /// <param name="response"></param>
        /// <param name="fileState"></param>
        /// <returns></returns>
        async Task<bool> CheckFileStateResponseWhenError(HttpResponse response, string fileState)
        {
            if (fileState == FileState.处理中)
            {
                await ResponseFile(response, Path.Combine(FileStateDir, $"{FileState.处理中}.jpg"));
            }
            else if (fileState == FileState.已删除)
            {
                await ResponseFile(response, Path.Combine(FileStateDir, $"{FileState.已删除}.jpg"));
            }
            else if (fileState != FileState.可用)
            {
                await ResponseFile(response, Path.Combine(FileStateDir, "不可用.jpg"));
            }
            else
                return true;

            return false;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="path"></param>
        static void DeleteFile(string path)
        {
            if (!File.Exists(path))
                return;

            File.Delete(path);
        }

        /// <summary>
        /// 获取MD5
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        static async Task<string> GetMD5(Stream stream)
        {
            using var md5 = MD5.Create();

            stream.Seek(0, SeekOrigin.Begin);
            var md5Byte = await md5.ComputeHashAsync(stream);
            var md5String = BitConverter.ToString(md5Byte);

            return md5String;
        }

        /// <summary>
        /// 获取文件状态
        /// </summary>
        /// <param name="md5">文件MD5值</param>
        /// <param name="chunk">是否为文件分片</param>
        /// <param name="specs">分片文件规格</param>
        /// <returns><see cref="Model.Common.FileState"/></returns>
        (string Id, string State, string Path) GetFileState(string md5, bool chunk, int? specs = null)
        {
            var state = chunk ? Repository_FileChunk.Where(o => o.MD5 == md5 && o.ServerKey == Config.ServerKey && o.Specs == specs.Value)
                                                            .OrderBy(o => o.State == $"{FileState.可用}")
                                                            .OrderBy(o => o.State == $"{FileState.上传中}")
                                                            .ToOne(o => new { o.Id, o.State, o.Path })
                            : Repository.Where(o => o.MD5 == md5 && o.ServerKey == Config.ServerKey)
                                        .OrderBy(o => o.State == $"{FileState.可用}")
                                        .OrderBy(o => o.State == $"{FileState.处理中}")
                                        .OrderBy(o => o.State == $"{FileState.上传中}")
                                        .ToOne(o => new { o.Id, o.State, o.Path });

            //Repository_FileChunk.Where(o => o.MD5 == md5 && o.ServerKey == Config.ServerKey && o.Specs == specs)
            //                                        .GroupBy(o => o.Index)
            //                                        .Count() == total ? new { Id = string.Empty, State = string.Empty, Path = string.Empty }

            return state?.State switch
            {
                FileState.可用 => (state.Id, FileState.可用, state.Path),
                FileState.处理中 => (state.Id, FileState.处理中, state.Path),
                FileState.上传中 => (state.Id, FileState.上传中, null),
                _ => (null, FileState.未上传, null)
            };
        }

        /// <summary>
        /// 获取文件信息
        /// </summary>
        /// <param name="md5">文件MD5值</param>
        /// <param name="state">状态<see cref="FileState"/></param>
        /// <returns></returns>
        Common_File GetFile(string md5, string state = null)
        {
            var file = Repository.Where(o => o.MD5 == md5 && o.ServerKey == Config.ServerKey && (state == null || o.State == $"{state}")).ToOne();
            return file;
        }

        /// <summary>
        /// 获取文件信息
        /// </summary>
        /// <param name="md5">文件MD5值</param>
        /// <param name="state">状态<see cref="FileState"/></param>
        /// <returns></returns>
        FileInfo GetFileInfo(string md5, string state = null)
        {
            var fileinfo = Mapper.Map<FileInfo>(GetFile(md5, state));
            return fileinfo;
        }

        /// <summary>
        /// 文件是否已存在
        /// </summary>
        /// <remarks>需要先计算MD5值</remarks>
        /// <param name="file"></param>
        /// <returns></returns>
        bool FileExists(FileInfo file)
        {
            if (file.MD5.IsNullOrWhiteSpace())
                throw new MessageException("请先计算文件MD5值.");

            var state = GetFileState(file.MD5, false);

            if (state.State != FileState.处理中 && state.State == FileState.可用)
                return false;

            var exists = Repository.Find(state.Id).InitEntity();
            file.State = exists.State;
            file.StorageType = exists.StorageType;
            file.FileType = exists.FileType;
            file.Extension = exists.Extension;
            file.ContentType = exists.ContentType;
            file.Path = exists.Path;
            file.Bytes = exists.Bytes;
            file.Size = exists.Size;

            return true;
        }

        /// <summary>
        /// 新增分片文件信息
        /// </summary>
        /// <param name="taskKey">任务标识</param>
        /// <param name="file_md5">文件MD5值</param>
        /// <param name="md5">分片文件MD5值</param>
        /// <param name="index">分片文件索引</param>
        /// <param name="specs">分片文件规格</param>
        /// <param name="path">路径</param>
        void AddChunkFile(string taskKey, string file_md5, string md5, int index, int specs, string path = null)
        {
            if (Repository_FileChunk.Where(o => o.TaskKey == taskKey && o.MD5 == md5).Any())
                return;

            var newData = new Common_ChunkFile
            {
                TaskKey = taskKey,
                ServerKey = Config.ServerKey,
                FileMD5 = file_md5,
                MD5 = md5,
                Index = index,
                Specs = specs,
                State = path == null ? FileState.上传中 : FileState.可用,
                Path = path
                //?? Repository_FileChunk.Where(o => o.MD5 == md5
                //                                    && o.ServerKey == Config.ServerKey
                //                                    && o.State == $"{FileState.可用}")
                //                                .ToOne(o => o.Path)
            }.InitEntity();

            Repository_FileChunk.Insert(newData);
        }

        /// <summary>
        /// 更新分片文件信息
        /// </summary>
        /// <param name="taskKey">任务标识</param>
        /// <param name="md5">分片MD5值</param>
        /// <param name="bytes">字节数</param>
        /// <param name="contentType">文件内容类型</param>
        /// <param name="extension">文件扩展名</param>
        /// <param name="path">路径</param>
        void UpdateChunkFile(string taskKey, string md5, long bytes, string contentType, string extension, string path)
        {
            if (!Repository_FileChunk.Where(o => o.TaskKey == taskKey && o.MD5 == md5).Any())
                throw new MessageException("分片文件不存在或已被移除.");

            var size = FileHelper.GetFileSize(bytes);

            if (Repository_FileChunk.UpdateDiy.Where(o => o.TaskKey == taskKey && o.MD5 == md5)
                                            .Set(o => o.Bytes, bytes)
                                            .Set(o => o.Size, size)
                                            .Set(o => o.ContentType, contentType)
                                            .Set(o => o.Extension, extension)
                                            .Set(o => o.Path, path)
                                            .Set(o => o.State, FileState.可用)
                                            .ExecuteAffrows() <= 0)
                throw new MessageException("更新分片文件信息失败.");
        }

        #endregion

        #region 外部接口

        #region 数据接口

        public List<FileInfo> GetList(PaginationDTO pagination)
        {
            var entityList = Orm.Select<Common_File>()
                        .Where(o => Operator.IsAdmin == true || o.CreatorId == Operator.AuthenticationInfo.Id)
                        .GetPagination(pagination)
                        .ToList<Common_File, FileInfo>(typeof(FileInfo).GetNamesWithTagAndOther(true, "_List"));

            var result = Mapper.Map<List<FileInfo>>(entityList);

            return result;
        }

        public FileInfo GetDetail(string id)
        {
            var result = Mapper.Map<FileInfo>(Repository.GetAndCheckNull(id));
            return result;
        }

        public List<FileInfo> GetDetails(string ids)
        {
            var list = GetList(new PaginationDTO { PageIndex = -1, DynamicFilterInfo = new List<PaginationDynamicFilterInfo> { new PaginationDynamicFilterInfo { Field = "Id", Compare = FilterCompare.inSet, Value = ids } } });
            return list;
        }

        public List<FileInfo> GetDetails(List<string> ids)
        {
            var list = GetList(new PaginationDTO { PageIndex = -1, DynamicFilterInfo = new List<PaginationDynamicFilterInfo> { new PaginationDynamicFilterInfo { Field = "Id", Compare = FilterCompare.inSet, Value = ids } } });
            return list;
        }

        #endregion

        #region 文件操作接口

        public ValidationMD5Response ValidationFileMD5(string md5, string filename = null)
        {
            var state = GetFileState(md5, false);

            var result = new ValidationMD5Response
            {
                Uploaded = state.State == FileState.处理中 || state.State == FileState.可用
            };

            if (result.Uploaded)
            {
                var file = Repository.Find(state.Id).InitEntity();

                file.Name = filename;

                Repository.Insert(file);

                result.FileInfo = Mapper.Map<FileInfo>(file);
            }

            return result;
        }

        public PreUploadChunkFileResponse PreUploadChunkFile(string file_md5, string md5, int index, int specs)
        {
            var file_state = GetFileState(file_md5, false);

            var result = new PreUploadChunkFileResponse
            {
                Key = Guid.NewGuid().ToString("N")
            };

            if (file_state.State == FileState.处理中 || file_state.State == FileState.可用)
                result.State = PUCFRState.全部跳过;
            else
            {
                var state = GetFileState(md5, true, specs);

                result.State = state.State switch
                {
                    FileState.可用 or FileState.处理中 => PUCFRState.跳过,
                    FileState.上传中 => PUCFRState.推迟上传,
                    _ => PUCFRState.允许上传
                };

                AddChunkFile(result.Key, file_md5, md5, index, specs, state.Path);
            }

            return result;
        }

        public async Task SingleChunkFile(string key, string md5, IFormFile file)
        {
            if (file == null)
                throw new MessageException("未上传任何文件.");

            var baseDirPath = Path.Combine(BaseDir, $"chunkfiles/{key}");

            if (!Directory.Exists(baseDirPath))
                Directory.CreateDirectory(baseDirPath);

            var path = Path.Combine(baseDirPath, $"{md5}.tmp");

            using var rs = file.OpenReadStream();
            await Save(rs, path);

            UpdateChunkFile(key, md5, rs.Length, file.ContentType, Path.GetExtension(file.FileName).ToLower(), path);
        }

        public FileInfo UploadChunkFileFinished(string file_md5, int specs, int total, string filename = null)
        {
            if (filename == null)
                filename = Guid.NewGuid().ToString();

            var file = new Common_File
            {
                ServerKey = Config.ServerKey,
                MD5 = file_md5,
                Name = filename,
                StorageType = StorageType.Path,
                State = FileState.处理中,
            }.InitEntity();

            Repository.Insert(file);

            MergeHandler.Add(file_md5, filename, specs, total);

            return Mapper.Map<FileInfo>(file);
        }

        public async Task<FileInfo> SingleImageFromBase64(string base64, string filename = null)
        {
            if (!Directory.Exists(BaseDir))
                Directory.CreateDirectory(BaseDir);

            var newData = new FileInfo
            {
                Name = filename ?? Guid.NewGuid().ToString(),
                StorageType = StorageType.Path
            };

            var image = ImgHelper.GetImgFromBase64Url(base64);
            using var ms = new MemoryStream();
            image.Save(ms);

            newData.MD5 = await GetMD5(ms);
            if (!FileExists(newData))
            {
                newData.FileType = FileType.图片;
                newData.Extension = ".jpg";
                newData.ContentType = "image/jpg";
                newData.Path = Path.Combine(BaseDir, $"{Guid.NewGuid()}{newData.Extension}");
                newData.Bytes = ms.Length;
                newData.Size = FileHelper.GetFileSize(ms.Length);

                await Save(ms, newData.Path);
            }

            newData.State = FileState.可用;

            newData.InitEntity();
            var entity = Mapper.Map<Common_File>(newData);
            Repository.Insert(entity);

            return newData;
        }

        public async Task<FileInfo> SingleFileFromUrl(string url, bool download = false, string filename = null)
        {
            if (!Directory.Exists(BaseDir))
                Directory.CreateDirectory(BaseDir);

            var newData = new FileInfo { Name = filename };

            if (download)
            {
                newData.Path = Path.Combine(BaseDir, $"{Guid.NewGuid()}");

                using var client = new WebClient();

                using var ms = new MemoryStream();
                var stream = await client.OpenReadTaskAsync(url);
                await stream.CopyToAsync(ms);

                newData.MD5 = await GetMD5(ms);

                if (!FileExists(newData))
                {
                    newData.ContentType = client.ResponseHeaders.Get("Content-Type") ?? "applicatoin/octet-stream";

                    var contentDisposition = client.ResponseHeaders.Get("Content-Disposition");
                    var index = contentDisposition?.IndexOf("filename=\"") ?? 0;
                    if (index > 0)
                    {
                        var fullName = contentDisposition[index..].TrimEnd('\"');
                        newData.Extension = Path.GetExtension(fullName);
                        if (newData.Name.IsNullOrWhiteSpace())
                            newData.Name = Path.GetFileNameWithoutExtension(fullName);
                    }

                    newData.StorageType = StorageType.Path;
                    newData.FileType = FileType.GetFileType(newData.Extension);
                    newData.Path = Path.Combine(BaseDir, $"{Guid.NewGuid()}{newData.Extension}");
                    newData.Bytes = ms.Length;
                    newData.Size = FileHelper.GetFileSize(ms.Length);

                    await Save(ms, newData.Path);
                }
            }
            else
            {
                newData.FileType = FileType.外链资源;
                newData.StorageType = StorageType.Uri;
                newData.Path = url;
                newData.State = FileState.可用;
            }

            newData.Name ??= Guid.NewGuid().ToString();

            newData.InitEntity();
            var entity = Mapper.Map<Common_File>(newData);
            Repository.Insert(entity);

            return newData;
        }

        public async Task<FileInfo> SingleFile(IFormFile file, string filename = null)
        {
            if (file == null)
                throw new MessageException("未上传任何文件.");

            if (!Directory.Exists(BaseDir))
                Directory.CreateDirectory(BaseDir);

            var newData = new FileInfo
            {
                Name = filename ?? file.Name,
                Extension = Path.GetExtension(file.FileName),
                ContentType = file.ContentType,
                StorageType = StorageType.Path
            };

            newData.FileType = FileType.GetFileType(newData.Extension);
            newData.Path = Path.Combine(BaseDir, $"{Guid.NewGuid()}{newData.Extension}");

            using var rs = file.OpenReadStream();
            await Save(rs, newData.Path);

            newData.Bytes = rs.Length;
            newData.Size = FileHelper.GetFileSize(newData.Bytes.Value);
            newData.State = FileState.可用;

            newData.InitEntity();
            var entity = Mapper.Map<Common_File>(newData);
            Repository.Insert(entity);

            return newData;
        }

        public async Task Preview(string id, int width, int height, TimeSpan? time = null)
        {
            var file = Repository.GetAndCheckNull(id, "文件不存在或已被删除.");

            var response = HttpContextAccessor.HttpContext.Response;

            if (!await CheckFileStateResponseWhenError(response, file.State))
                return;

            if (file.StorageType == StorageType.Uri)
            {
                response.Redirect(file.Path);
                return;
            }
            else if (file.StorageType != StorageType.Path)
                throw new MessageException("此文件不支持预览.");

            if (!FileHelper.Exists(file.Path))
                throw new MessageException("文件已被删除.");

            if (file.FileType == FileType.图片)
            {
                var imagePath = $"{file.Path.Replace(file.Extension, "")}-Screenshot/{width}x{height}.jpg";

                if (!imagePath.Exists())
                {
                    if (!Directory.Exists(imagePath))
                        Directory.CreateDirectory(imagePath);

                    using var fs = new FileStream(file.Path, FileMode.Open, FileAccess.Read);
                    var image = new Image(fs);

                    using var newFS = File.Create(imagePath);
                    image.Resize(width, height)
                       .Save(newFS);
                }

                response.ContentType = file.ContentType;
                await ResponseFile(response, imagePath);
            }
            else if (file.FileType == FileType.视频)
            {
                var imagePath = $"{file.Path.Replace(file.Extension, "")}-Screenshot/{width}x{height}.jpg";

                if (!imagePath.Exists())
                {
                    try
                    {
                        await file.Path.Screenshot(imagePath, time ?? TimeSpan.FromSeconds(1), 31, width, height);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(
                            NLog.LogLevel.Warn,
                            LogType.警告信息,
                            "视频文件截图失败.",
                            $"File: {file.Path}, Time: {time ?? TimeSpan.FromSeconds(1)}, quality: 31, width: {width}, height: {height}.",
                            ex);

                        response.ContentType = "image/jpg";
                        await ResponseFile(response, GetPreviewImage(file.Extension));
                        return;
                    }
                }

                response.ContentType = "image/jpg";
                await ResponseFile(response, imagePath);
            }
            else
            {
                response.ContentType = "image/jpg";
                await ResponseFile(response, GetPreviewImage(file.Extension));
            }

            //throw new MessageException("此文件不支持预览.");
        }

        public async Task Browse(string id)
        {
            var file = Repository.GetAndCheckNull(id, "文件不存在或已被删除.");

            var response = HttpContextAccessor.HttpContext.Response;

            if (!await CheckFileStateResponseWhenError(response, file.State))
                return;

            if (file.StorageType == StorageType.Uri)
            {
                response.Redirect(file.Path);
                return;
            }
            else if (file.StorageType != StorageType.Path)
                throw new MessageException("此文件不支持浏览.");

            response.ContentType = file.ContentType;
            await ResponseFile(response, file.Path);
        }

        public async Task Download(string id)
        {
            var file = Repository.GetAndCheckNull(id, "文件不存在或已被删除.");

            var response = HttpContextAccessor.HttpContext.Response;

            if (!await CheckFileStateResponseWhenError(response, file.State))
                return;

            if (file.StorageType == StorageType.Uri)
            {
                response.Redirect(file.Path);
                return;
            }
            else if (file.StorageType != StorageType.Path)
                throw new MessageException("此文件不支持下载.");

            response.ContentType = file.ContentType;
            //response.ContentType = "applicatoin/octet-stream";
            response.Headers.Add("Content-Disposition", $"attachment; filename=\"{UrlEncoder.Default.Encode($"{file.Name}{file.Extension}")}\"");

            await ResponseFile(response, file.Path);
        }

        public void Delete(List<string> ids)
        {
            var files = Repository.Select.Where(c => ids.Contains(c.Id)).ToList(c => new { c.Id, c.Path });

            foreach (var file in files)
            {
                if (!file.Path.IsNullOrEmpty())
                    DeleteFile(file.Path);
            }

            if (Repository.Delete(o => ids.Contains(o.Id)) <= 0)
                throw new MessageException("未删除任何数据.");
        }

        #endregion

        #endregion
    }
}

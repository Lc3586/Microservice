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
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
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
            IHttpContextAccessor httpContextAccessor)
        {
            Orm = freeSqlProvider.GetFreeSql();
            Mapper = autoMapperProvider.GetMapper();
            Repository = Orm.GetRepository<Common_File, string>();
            Repository_FileChunk = Orm.GetRepository<Common_ChunkFile, string>();
            Repository_FileExtension = Orm.GetRepository<Common_FileExtension, string>();
            HttpContextAccessor = httpContextAccessor;
            HttpRequest = HttpContextAccessor?.HttpContext?.Request;
            HttpResponse = HttpContextAccessor?.HttpContext?.Response;

            if (Config.EnableUploadLargeFile)
                MergeHandler = AutofacHelper.GetService<ChunkFileMergeHandler>();

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

        readonly IBaseRepository<Common_FileExtension, string> Repository_FileExtension;

        readonly IHttpContextAccessor HttpContextAccessor;

        readonly HttpRequest HttpRequest;

        readonly HttpResponse HttpResponse;

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
        /// 获取文件类型预览图
        /// </summary>
        /// <param name="extension">文件后缀</param>
        /// <returns></returns>
        string GetFileTypeImage(string extension = null)
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
        /// 输出错误
        /// </summary>
        /// <param name="fileState"></param>
        /// <returns></returns>
        async Task<bool> CheckFileStateResponseWhenError(string fileState)
        {
            if (fileState == FileState.处理中)
            {
                await ResponseFile(HttpRequest, HttpResponse, Path.Combine(FileStateDir, $"{FileState.处理中}.jpg"));
            }
            else if (fileState == FileState.已删除)
            {
                await ResponseFile(HttpRequest, HttpResponse, Path.Combine(FileStateDir, $"{FileState.已删除}.jpg"));
            }
            else if (fileState != FileState.可用)
            {
                await ResponseFile(HttpRequest, HttpResponse, Path.Combine(FileStateDir, "不可用.jpg"));
            }
            else
                return true;

            return false;
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
                                                            .OrderByDescending(o => o.State == $"{FileState.可用}" ? 1 : 0)
                                                            .OrderByDescending(o => o.State == $"{FileState.上传中}" ? 1 : 0)
                                                            .ToOne(o => new { o.Id, o.State, o.Path })
                            : Repository.Where(o => o.MD5 == md5 && o.ServerKey == Config.ServerKey)
                                        .OrderByDescending(o => o.State == $"{FileState.可用}" ? 1 : 0)
                                        .OrderByDescending(o => o.State == $"{FileState.处理中}" ? 1 : 0)
                                        .OrderByDescending(o => o.State == $"{FileState.上传中}" ? 1 : 0)
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
        /// <param name="md5">文件MD5值</param>
        /// <param name="file">文件信息</param>
        /// <returns></returns>
        bool FileExists(string md5, out Common_File file)
        {
            file = null;

            if (md5.IsNullOrWhiteSpace())
                throw new MessageException("请先计算文件MD5值.", new { MD5 = md5 });

            var state = GetFileState(md5, false);

            if (state.State != FileState.处理中 && state.State != FileState.可用)
                return false;

            file = Repository.Find(state.Id);

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
        /// <param name="path">路径</param>
        void UpdateChunkFile(string taskKey, string md5, long bytes, /*string contentType, string extension,*/ string path)
        {
            if (!Repository_FileChunk.Where(o => o.TaskKey == taskKey && o.MD5 == md5).Any())
                throw new MessageException("分片文件不存在或已被移除.", new { TaskKey = taskKey, MD5 = md5 });

            var size = FileHelper.GetFileSize(bytes);

            if (Repository_FileChunk.UpdateDiy.Where(o => o.TaskKey == taskKey && o.MD5 == md5)
                                            .Set(o => o.Bytes, bytes)
                                            .Set(o => o.Size, size)
                                            //.Set(o => o.ContentType, contentType)
                                            //.Set(o => o.Extension, extension)
                                            .Set(o => o.Path, path)
                                            .Set(o => o.State, FileState.可用)
                                            .ExecuteAffrows() <= 0)
                throw new MessageException("更新分片文件信息失败.", new { TaskKey = taskKey, MD5 = md5 });
        }

        /// <summary>
        /// 上传单个分片文件
        /// </summary>
        /// <param name="key">上传标识</param>
        /// <param name="md5">分片文件MD5值</param>
        /// <param name="stream"></param>
        /// <returns></returns>
        async Task SingleChunkFile(string key, string md5, Stream stream)
        {
            var baseDirPath = Path.Combine(BaseDir, $"chunkfiles/{key}");

            if (!Directory.Exists(baseDirPath))
                Directory.CreateDirectory(baseDirPath);

            var path = Path.Combine(baseDirPath, $"{md5}.tmp");

            await Save(stream, path);

            if (!path.Exists())
                throw new MessageException("未上传任何文件.");

            var md5_ = await GetMD5(path);
            if (md5_ != md5)
            {
                File.Delete(path);
                throw new MessageException("MD5值未通过验证.");
            }

            UpdateChunkFile(key, md5, stream.Length, /*file.ContentType, Path.GetExtension(filename).ToLower(),*/ path);
        }

        /// <summary>
        /// 上传单个文件
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="type">文件类型</param>
        /// <param name="extension">文件拓展名</param>
        /// <param name="filename">文件重命名</param>
        /// <returns></returns>
        async Task<FileInfo> SingleFile(Stream stream, string type, string extension, string filename)
        {
            if (!Directory.Exists(BaseDir))
                Directory.CreateDirectory(BaseDir);

            var name = Guid.NewGuid().ToString();
            var path = Path.Combine(BaseDir, $"{name}{extension}");

            await Save(stream, path);

            if (!path.Exists())
                throw new MessageException("未上传任何文件.");

            var md5 = await GetMD5(path);

            var validation = PreUploadFile(md5, filename);
            if (validation.Uploaded)
            {
                File.Delete(path);
                return validation.FileInfo;
            }

            var file = new Common_File
            {
                ServerKey = Config.ServerKey,
                MD5 = md5,
                Name = name,
                ContentType = type,
                Extension = extension,
                FileType = FileType.GetFileTypeByExtension(extension),
                Bytes = path.GetFileBytes(),
                Path = path,
                StorageType = StorageType.Path,
                State = FileState.可用,
            }.InitEntity();

            file.Size = FileHelper.GetFileSize(file.Bytes.Value);

            var file_extension = new Common_FileExtension
            {
                Name = filename,
                FileId = file.Id,
                State = FileExtensionState.可用
            }.InitEntity();

            void handler()
            {
                Repository.Insert(file);

                Repository_FileExtension.Insert(file_extension);
            }

            (bool success, Exception ex) = Orm.RunTransaction(handler);

            if (!success)
                throw new MessageException("数据处理失败.", ex);

            return GetDetail(file_extension.Id);
        }

        #endregion

        #region 外部接口

        #region 数据接口

        public List<FileInfo> GetList(PaginationDTO pagination)
        {
            var result = Orm.Select<Common_FileExtension>()
                        .Where(o => Operator.IsAdmin == true || o.CreatorId == Operator.AuthenticationInfo.Id)
                        .GetPagination(pagination)
                        .ToList(o => new FileInfo
                        {
                            Id = o.Id,
                            Name = o.Name,
                            FileType = o.Common_File.FileType,
                            ContentType = o.Common_File.ContentType,
                            Extension = o.Common_File.Extension,
                            MD5 = o.Common_File.MD5,
                            ServerKey = o.Common_File.ServerKey,
                            StorageType = o.Common_File.StorageType,
                            Size = o.Common_File.Size,
                            State = o.Common_File.State,
                            CreateTime = o.Common_File.CreateTime,
                        });

            return result;
        }

        public FileInfo GetDetail(string id)
        {
            var result = Orm.Select<Common_FileExtension>()
                        .Where(o => o.Id == id)
                        .ToOne(o => new FileInfo
                        {
                            Id = o.Id,
                            Name = o.Name,
                            FileType = o.Common_File.FileType,
                            ContentType = o.Common_File.ContentType,
                            Extension = o.Common_File.Extension,
                            MD5 = o.Common_File.MD5,
                            ServerKey = o.Common_File.ServerKey,
                            StorageType = o.Common_File.StorageType,
                            Size = o.Common_File.Size,
                            State = o.Common_File.State,
                            CreateTime = o.Common_File.CreateTime,
                        });

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

        public void Rename(string id, string filename)
        {
            var file_extension = Repository_FileExtension.GetAndCheckNull(id, "文件不存在或已被删除.");
            file_extension.Name = filename;
            file_extension.ModifyEntity();
            Repository_FileExtension.Update(file_extension);
        }

        #endregion

        #region 文件操作接口

        public string GetFileTypeByExtension(string extension)
        {
            return FileType.GetFileTypeByExtension($".{extension.ToLower().TrimStart('.')}");
        }

        public string GetFileTypeByMIME(string mimetype)
        {
            return FileType.GetFileTypeByMIME(mimetype.ToLower());
        }

        public async Task GetFileTypeImageUrl(string extension)
        {
            var response = HttpContextAccessor.HttpContext.Response;
            response.ContentType = "image/jpg";
            await ResponseFile(HttpRequest, HttpResponse, GetFileTypeImage(extension.ToLower()));
        }

        public string GetFileSize(string length)
        {
            if (!long.TryParse(length, out long value))
                throw new MessageException("length值格式有误.");
            return value.GetFileSize();
        }

        public PreUploadFileResponse PreUploadFile(string md5, string filename, bool section = false, string type = null, string extension = null, int? specs = null, int? total = null)
        {
            var state = GetFileState(md5, false);

            var result = new PreUploadFileResponse
            {
                Uploaded = state.State == FileState.处理中 || state.State == FileState.可用
            };

            if (result.Uploaded)
            {
                var file_extension_id = Repository_FileExtension
                                        .Where(o =>
                                            o.FileId == state.Id
                                            && o.Name == filename
                                            && o.State == $"{FileExtensionState.可用}"
                                            && o.CreatorId == Operator.AuthenticationInfo.Id)
                                        .ToOne(o => o.Id);

                if (file_extension_id == default)
                {
                    var file_extension = new Common_FileExtension
                    {
                        Name = filename,
                        FileId = state.Id,
                        State = FileExtensionState.可用
                    }.InitEntity();

                    file_extension_id = file_extension.Id;

                    Repository_FileExtension.Insert(file_extension);
                }

                result.FileInfo = GetDetail(file_extension_id);
            }
            else if (section)
            {
                if (!Config.EnableUploadLargeFile)
                    throw new MessageException("未启用大文件上传功能.");

                MergeHandler.Add(md5, type, extension, filename, specs.Value, total.Value);
            }

            return result;
        }

        public PreUploadChunkFileResponse PreUploadChunkFile(string file_md5, string md5, int index, int specs, bool forced = false)
        {
            if (!Config.EnableUploadLargeFile)
                throw new MessageException("未启用大文件上传功能.");

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

                if (forced)
                    result.State = PUCFRState.允许上传;
                else
                    result.State = state.State switch
                    {
                        FileState.可用 or FileState.处理中 => PUCFRState.跳过,
                        FileState.上传中 => PUCFRState.推迟上传,
                        _ => PUCFRState.允许上传
                    };

                if (result.State == PUCFRState.允许上传)
                    AddChunkFile(result.Key, file_md5, md5, index, specs, state.Path);
            }

            return result;
        }

        public async Task SingleChunkFile(string key, string md5, IFormFile file)
        {
            if (!Config.EnableUploadLargeFile)
                throw new MessageException("未启用大文件上传功能.");

            if (file == null)
                throw new MessageException("未上传任何文件.", new { Key = key, MD5 = md5 });

            using var rs = file.OpenReadStream();
            await SingleChunkFile(key, md5, rs);
        }

        public async Task SingleChunkFileByArrayBuffer(string key, string md5)
        {
            if (!Config.EnableUploadLargeFile)
                throw new MessageException("未启用大文件上传功能.");

            using var rs = HttpContextAccessor.HttpContext.Request.Body;
            await SingleChunkFile(key, md5, rs);
        }

        public FileInfo UploadChunkFileFinished(string file_md5, int specs, int total, string type, string extension, string filename)
        {
            if (!Config.EnableUploadLargeFile)
                throw new MessageException("未启用大文件上传功能.");

            var file_state = GetFileState(file_md5, false);

            Common_File file = null;

            var file_extension = new Common_FileExtension
            {
                Name = filename,
                State = FileExtensionState.可用
            };

            if (file_state.State == FileState.可用 || file_state.State == FileState.处理中)
            {
                file_extension.FileId = file_state.Id;

                file_extension.Id = Repository_FileExtension
                                        .Where(o =>
                                            o.FileId == file_state.Id
                                            && o.Name == filename
                                            && o.State == $"{FileExtensionState.可用}"
                                            && o.CreatorId == Operator.AuthenticationInfo.Id)
                                        .ToOne(o => o.Id);

                if (file_extension.Id != default)
                    goto end;
            }
            else
            {
                var name = Guid.NewGuid().ToString();
                file = new Common_File
                {
                    ServerKey = Config.ServerKey,
                    MD5 = file_md5,
                    Name = name,
                    ContentType = type,
                    Extension = extension,
                    StorageType = StorageType.Path,
                    State = FileState.处理中,
                }.InitEntity();

                file_extension.FileId = file.Id;
            }

            file_extension.InitEntity();

            void handler()
            {
                if (file != null)
                    Repository.Insert(file);

                Repository_FileExtension.Insert(file_extension);
            }

            (bool success, Exception ex) = Orm.RunTransaction(handler);

            if (!success)
                throw new MessageException("数据处理失败.", ex);

            MergeHandler.Handler(file_md5, specs, total);

            end:
            return GetDetail(file_extension.Id);
        }

        public async Task<FileInfo> SingleImageFromBase64(string base64, string filename)
        {
            if (!Directory.Exists(BaseDir))
                Directory.CreateDirectory(BaseDir);

            var name = Guid.NewGuid().ToString();
            var path = Path.Combine(BaseDir, $"{name}.bmp");

            var image = ImgHelper.GetImgFromBase64Url(base64);
            Save(image, path);
            var md5 = await GetMD5(path);

            if (!FileExists(md5, out Common_File file))
            {

                file = new Common_File
                {
                    Name = name,
                    StorageType = StorageType.Path,
                    FileType = FileType.图片,
                    Extension = ".bmp",
                    ContentType = "image/x-ms-bmp",
                    Path = path,
                    Bytes = path.GetFileBytes(),
                    State = FileState.可用
                }.InitEntity();

                file.Size = FileHelper.GetFileSize(file.Bytes.Value);
            }
            else
                File.Delete(path);

            var file_extension = new Common_FileExtension
            {
                Name = filename,
                FileId = file.Id,
                State = FileExtensionState.可用
            }.InitEntity();

            void handler()
            {
                Repository.Insert(file);

                Repository_FileExtension.Insert(file_extension);
            }

            (bool success, Exception ex) = Orm.RunTransaction(handler);

            if (!success)
                throw new MessageException("数据处理失败.", ex);

            return GetDetail(file_extension.Id);
        }

        public async Task<FileInfo> SingleFileFromUrl(string url, string filename, bool download = false)
        {
            if (!Directory.Exists(BaseDir))
                Directory.CreateDirectory(BaseDir);

            var name = Guid.NewGuid().ToString();

            Common_File file;

            if (download)
            {
                var extension = url.LastIndexOf('/') >= 0 ? Path.GetExtension(url) : string.Empty;

                var path = Path.Combine(BaseDir, $"{name}{extension}");

                using var client = new WebClient();
                var stream = await client.OpenReadTaskAsync(url);

                var contentType = client.ResponseHeaders[HttpResponseHeader.ContentType];

                await Save(stream, path);

                var md5 = await GetMD5(path);

                if (!FileExists(md5, out Common_File _file))
                {
                    file = new Common_File
                    {
                        Name = name,
                        StorageType = StorageType.Path,
                        FileType = contentType.IsNullOrWhiteSpace() ? GetFileTypeByExtension(extension) : GetFileTypeByMIME(contentType),
                        Extension = extension,
                        ContentType = contentType,
                        Path = path,
                        Bytes = path.GetFileBytes(),
                        State = FileState.可用
                    }.InitEntity();

                    file.Size = FileHelper.GetFileSize(file.Bytes.Value);
                }
                else
                {
                    file = _file;
                    File.Delete(path);
                }
            }
            else
            {
                var _file = Repository.Where(o => o.StorageType == $"{StorageType.Uri}" && o.Path == url && o.State == $"{FileState.可用}").ToOne();
                if (_file != null)
                    file = _file;
                else
                    file = new Common_File
                    {
                        Name = name,
                        FileType = FileType.未知,
                        StorageType = StorageType.Uri,
                        Path = url,
                        State = FileState.可用
                    }.InitEntity();
            }

            var file_extension = new Common_FileExtension
            {
                Name = filename,
                FileId = file.Id,
                State = FileExtensionState.可用
            }.InitEntity();

            void handler()
            {
                Repository.Insert(file);

                Repository_FileExtension.Insert(file_extension);
            }

            (bool success, Exception ex) = Orm.RunTransaction(handler);

            if (!success)
                throw new MessageException("数据处理失败.", ex);

            return GetDetail(file_extension.Id);
        }

        public async Task<FileInfo> SingleFile(IFormFile file, string filename)
        {
            if (file == null)
                throw new MessageException("未上传任何文件.", filename);

            using var rs = file.OpenReadStream();
            return await SingleFile(rs, file.ContentType, Path.GetExtension(file.FileName), filename);
        }

        public async Task<FileInfo> SingleFileByArrayBuffer(string type, string extension, string filename)
        {
            using var rs = HttpContextAccessor.HttpContext.Request.Body;

            return await SingleFile(rs, type, extension, filename);
        }

        public async Task Preview(string id, int width, int height, TimeSpan? time = null)
        {
            var response = HttpContextAccessor.HttpContext.Response;

            var file_extension = Repository_FileExtension.Get(id);
            if (file_extension == null)
            {
                Response(response, StatusCodes.Status404NotFound, "文件不存在或已被删除.");
                return;
            }

            var file = Repository.Get(file_extension.FileId);
            if (file == null)
            {
                Response(response, StatusCodes.Status404NotFound, "文件不存在或已被删除.");
                return;
            }

            if (!await CheckFileStateResponseWhenError(file.State))
                return;

            if (file.StorageType == StorageType.Uri)
            {
                response.Redirect(file.Path);
                return;
            }
            else if (file.StorageType != StorageType.Path)
            {
                Response(response, StatusCodes.Status412PreconditionFailed, "此文件不支持预览.");
                return;
            }

            if (!FileHelper.Exists(file.Path))
            {
                Response(response, StatusCodes.Status404NotFound, "文件不存在或已被删除.");
                return;
            }
            //throw new MessageException("文件已被删除.", file.Path);

            if (file.FileType == FileType.图片)
            {
                var imagePath = $"{(file.Extension.IsNullOrWhiteSpace() ? file.Path : file.Path.Replace(file.Extension, ""))}-Screenshot/{width}x{height}{file.Extension}";

                if (!imagePath.Exists())
                {
                    var dir = imagePath.Substring(0, imagePath.LastIndexOf('/'));
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    try
                    {
                        using var fs = new FileStream(file.Path, FileMode.Open, FileAccess.Read);
                        var image = new Image(fs);

                        using var newFS = File.Create(imagePath);
                        image.Resize(width, height)
                           .Save(newFS);
                    }
                    catch (Exception ex)
                    {
                        imagePath = file.Path;

                        Logger.Log(
                            NLog.LogLevel.Warn,
                            LogType.警告信息,
                            "图片截图失败.",
                            file.Path,
                            ex);
                    }
                }

                response.ContentType = file.ContentType.IsNullOrWhiteSpace() ? "applicatoin/octet-stream" : file.ContentType;
                await ResponseFile(HttpRequest, HttpResponse, imagePath);
            }
            else if (file.FileType == FileType.视频)
            {
                time ??= TimeSpan.FromSeconds(0.001);
                var imagePath = $"{(file.Extension.IsNullOrWhiteSpace() ? file.Path : file.Path.Replace(file.Extension, ""))}-Screenshot/{width}x{height}-{time.Value:c}.jpg";

                if (!File.Exists(imagePath))
                {
                    try
                    {
                        var filePath = Config.GetFileAbsolutePath("ffmpeg");

                        await file.Path.Screenshot(imagePath, filePath, time.Value, 31, width, height);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(
                            NLog.LogLevel.Warn,
                            LogType.警告信息,
                            "视频文件截图失败.",
                            $"File: {file.Path}, Time: {time.Value:c}, quality: 31, width: {width}, height: {height}.",
                            ex);

                        response.ContentType = "image/jpg";
                        await ResponseFile(HttpRequest, HttpResponse, GetFileTypeImage(file.Extension));
                        return;
                    }
                }

                response.ContentType = "image/jpg";
                await ResponseFile(HttpRequest, HttpResponse, imagePath);
            }
            else
            {
                response.ContentType = "image/jpg";
                await ResponseFile(HttpRequest, HttpResponse, GetFileTypeImage(file.Extension));
            }

            //throw new MessageException("此文件不支持预览.");
        }

        public async Task Browse(string id)
        {
            var response = HttpContextAccessor.HttpContext.Response;

            var file_extension = Repository_FileExtension.Get(id);
            if (file_extension == null)
            {
                Response(response, StatusCodes.Status404NotFound, "文件不存在或已被删除.");
                return;
            }

            var file = Repository.Get(file_extension.FileId);
            if (file == null)
            {
                Response(response, StatusCodes.Status404NotFound, "文件不存在或已被删除.");
                return;
            }

            if (!await CheckFileStateResponseWhenError(file.State))
                return;

            if (file.StorageType == StorageType.Uri)
            {
                response.Redirect(file.Path);
                return;
            }
            else if (file.StorageType != StorageType.Path)
            {
                Response(response, StatusCodes.Status412PreconditionFailed, "此文件不支持浏览.");
                return;
            }
            else if (file.FileType != FileType.图片 && file.FileType != FileType.视频 && file.FileType != FileType.文本文件 && file.ContentType != "application/pdf")
            {
                Response(response, StatusCodes.Status412PreconditionFailed, "此文件不支持浏览.");
                return;
            }

            response.ContentType = file.ContentType.IsNullOrWhiteSpace() ? "applicatoin/octet-stream" : file.ContentType;

            await ResponseFile(HttpRequest, HttpResponse, file.Path, file.Bytes);
        }

        public async Task Download(string id)
        {
            var response = HttpContextAccessor.HttpContext.Response;

            var file_extension = Repository_FileExtension.Get(id);
            if (file_extension == null)
            {
                Response(response, StatusCodes.Status404NotFound, "文件不存在或已被删除.");
                return;
            }

            var file = Repository.Get(file_extension.FileId);
            if (file == null)
            {
                Response(response, StatusCodes.Status404NotFound, "文件不存在或已被删除.");
                return;
            }

            if (!await CheckFileStateResponseWhenError(file.State))
                return;

            if (file.StorageType == StorageType.Uri)
            {
                response.Redirect(file.Path);
                return;
            }
            else if (file.StorageType != StorageType.Path)
            {
                Response(response, StatusCodes.Status412PreconditionFailed, "此文件不支持下载.");
                return;
            }

            response.ContentType = file.ContentType.IsNullOrWhiteSpace() ? "applicatoin/octet-stream" : file.ContentType;
            response.Headers.Add("Content-Disposition", $"attachment; filename=\"{UrlEncoder.Default.Encode($"{file_extension.Name}{file.Extension}")}\"");

            await ResponseFile(HttpRequest, HttpResponse, file.Path, file.Bytes);
        }

        public void Delete(List<string> ids)
        {
            var files = Repository_FileExtension.Select
                .Where(c => ids.Contains(c.Id))
                .ToList(c => new
                {
                    c.Id,
                    c.Name,
                    c.Common_File.StorageType,
                    c.Common_File.Path,
                    c.Common_File.Extension
                });

            foreach (var file in files)
            {
                if (file.StorageType != StorageType.Path || file.Path.IsNullOrEmpty())
                    continue;

                //原文件
                DeleteFile(file.Path);

                //缩略图&截图
                var s = $"{(file.Extension.IsNullOrWhiteSpace() ? file.Path : file.Path.Replace(file.Extension, ""))}-Screenshot";
                if (Directory.Exists(s))
                    Directory.Delete(s, true);
            }

            if (Repository_FileExtension.Delete(o => ids.Contains(o.Id)) <= 0)
                throw new MessageException("未删除任何数据.", ids);
        }

        #endregion

        #region 通用方法

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="image">图像</param>
        /// <param name="path">绝对路径</param>
        public static void Save(Image image, string path)
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
        public static async Task Save(byte[] bytes, string path)
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
        public static async Task Save(Stream stream, string path)
        {
            try
            {
                if (stream.CanSeek)
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
        /// 输出
        /// </summary>
        /// <param name="response">Http响应对象</param>
        /// <param name="statusCode">状态码</param>
        /// <param name="message">消息</param>
        public static void Response(HttpResponse response, int statusCode, string message)
        {
            response.StatusCode = statusCode;
            response.ContentType = "text/plan; charset=utf-8";
            response.WriteAsync(message);
        }

        /// <summary>
        /// 输出图片
        /// </summary>
        /// <param name="request">Http请求对象</param>
        /// <param name="response">Http响应对象</param>
        /// <param name="img"></param>
        public static async Task ResponseImage(HttpRequest request, HttpResponse response, Image img)
        {
            using var ms = new MemoryStream();
            img.Save(ms);
            await Response(request, response, ms);
        }

        /// <summary>
        /// 输出
        /// </summary>
        /// <param name="request">Http请求对象</param>
        /// <param name="response">Http响应对象</param>
        /// <param name="stream">流</param>
        public static async Task Response(HttpRequest request, HttpResponse response, Stream stream)
        {
            if (IsResponseRangeBytes(request, response, stream.Length, out long start, out long count))
            {
                if (stream.CanSeek)
                    stream.Seek(start, SeekOrigin.Begin);
                var buffer = new byte[1024];
                int wrote = 0;
                while (wrote <= count)
                {
                    var length = await stream.ReadAsync(buffer.AsMemory(0, 1024));
                    await response.Body.WriteAsync(buffer.AsMemory(0, length));
                    wrote += length;
                    Array.Clear(buffer, 0, length);
                }
            }
            else
                await stream.CopyToAsync(response.Body);
        }

        /// <summary>
        /// 输出文件
        /// </summary>
        /// <param name="request">Http请求对象</param>
        /// <param name="response">Http响应对象</param>
        /// <param name="path">文件路径</param>
        /// <param name="bytes">总字节数</param>
        public static async Task ResponseFile(HttpRequest request, HttpResponse response, string path, long? bytes = null)
        {
            if (!File.Exists(path))
            {
                Response(response, StatusCodes.Status404NotFound, "文件不存在或已被删除.");
                return;
            }
            //throw new MessageException($"文件不存在或已被删除.", path);

            if (!bytes.HasValue)
                bytes = FileHelper.GetFileBytes(path);

            if (IsResponseRangeBytes(request, response, bytes.Value, out long start, out long count))
                await response.SendFileAsync(path, start, count);
            else
                await response.SendFileAsync(path);
        }

        /// <summary>
        /// 是否需要分段输出数据
        /// </summary>
        /// <param name="request">Http请求对象</param>
        /// <param name="response">Http响应对象</param>
        /// <param name="bytes">总字节数</param>
        /// <param name="start">开始位置</param>
        /// <param name="count">输出字节数</param>
        /// <returns></returns>
        public static bool IsResponseRangeBytes(HttpRequest request, HttpResponse response, long bytes, out long start, out long count)
        {
            if (!request.Headers.TryGetValue("Range", out StringValues value))
            {
                start = 0;
                count = bytes;
                response.StatusCode = StatusCodes.Status200OK;
                response.ContentLength = bytes;
                return false;
            }

            var rangeValue = value.ToString().Replace("bytes=", "");
            var range = rangeValue.Split('-');
            start = range[0].ToLong();
            count = 0;

            if (range.Length > 1)
                count = range[1].ToLong() - start;

            if (count == 0 || start + count > bytes)
                count = bytes - start;

            response.StatusCode = StatusCodes.Status206PartialContent;
            response.Headers.Add("Accept-Ranges", "bytes");
            response.Headers.Add("Content-Range", $"bytes {start}-{start + count - 1}/{bytes}");
            response.ContentLength = count;

            return true;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="path"></param>
        public static void DeleteFile(string path)
        {
            if (!File.Exists(path))
                return;

            File.Delete(path);
        }

        /// <summary>
        /// 获取MD5
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async Task<string> GetMD5(string path)
        {
            using var fs = File.OpenRead(path);
            return await GetMD5(fs);
        }

        /// <summary>
        /// 获取MD5
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static async Task<string> GetMD5(Stream stream)
        {
            using var md5 = MD5.Create();

            if (stream.CanSeek)
                stream.Seek(0, SeekOrigin.Begin);
            var md5Byte = await md5.ComputeHashAsync(stream);
            var md5String = BitConverter.ToString(md5Byte);

            return md5String.Replace("-", string.Empty).ToLower();
        }

        #endregion

        #endregion
    }
}

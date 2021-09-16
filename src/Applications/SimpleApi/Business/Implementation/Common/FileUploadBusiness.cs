using Business.Handler;
using Business.Interface.Common;
using Business.Interface.System;
using Business.Utils;
using Entity.Common;
using FreeSql;
using Microservice.Library.Container;
using Microservice.Library.Extension;
using Microservice.Library.File;
using Microservice.Library.FreeSql.Extention;
using Microservice.Library.FreeSql.Gen;
using Microsoft.AspNetCore.Http;
using Model.Common;
using Model.Common.FileUploadDTO;
using Model.Common.PersonalFileInfoDTO;
using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Business.Implementation.Common
{
    /// <summary>
    /// 文件上传业务类
    /// </summary>
    public class FileUploadBusiness : BaseBusiness, IFileUploadBusiness, IDependency
    {
        #region DI

        public FileUploadBusiness(
            IFreeSqlProvider freeSqlProvider,
            IHttpContextAccessor httpContextAccessor,
            IFileBusiness fileBusiness,
            IPersonalFileInfoBusiness personalFileInfoBusiness,
            IAuthoritiesBusiness authoritiesBusiness)
        {
            Orm = freeSqlProvider.GetFreeSql();
            Repository = Orm.GetRepository<Common_File, string>();
            Repository_FileChunk = Orm.GetRepository<Common_ChunkFile, string>();
            Repository_FileExtension = Orm.GetRepository<Common_PersonalFileInfo, string>();
            Repository_FileUploadConfig = Orm.GetRepository<Common_FileUploadConfig, string>();
            HttpContextAccessor = httpContextAccessor;
            FileBusiness = fileBusiness;
            PersonalFileInfoBusiness = personalFileInfoBusiness;
            AuthoritiesBusiness = authoritiesBusiness;

            if (Config.EnableUploadLargeFile)
                MergeHandler = AutofacHelper.GetService<ChunkFileMergeHandler>();

            BaseDir = $"{Config.AbsoluteStorageDirectory}/upload/{DateTime.Now:yyyy-MM-dd}";
        }

        #endregion

        #region 私有成员

        readonly IFreeSql Orm;

        readonly IBaseRepository<Common_File, string> Repository;

        readonly IBaseRepository<Common_ChunkFile, string> Repository_FileChunk;

        readonly IBaseRepository<Common_PersonalFileInfo, string> Repository_FileExtension;

        readonly IBaseRepository<Common_FileUploadConfig, string> Repository_FileUploadConfig;

        readonly IHttpContextAccessor HttpContextAccessor;

        readonly IFileBusiness FileBusiness;

        readonly IPersonalFileInfoBusiness PersonalFileInfoBusiness;

        readonly IAuthoritiesBusiness AuthoritiesBusiness;

        readonly ChunkFileMergeHandler MergeHandler;

        /// <summary>
        /// 存储路径根目录绝对路径
        /// </summary>
        readonly string BaseDir;

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

            await Common.FileBusiness.Save(stream, path);

            if (!path.Exists())
                throw new MessageException("未上传任何文件.");

            var md5_ = await Common.FileBusiness.GetMD5(path);
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
        /// <param name="configId">上传配置Id</param>
        /// <param name="stream"></param>
        /// <param name="type">文件类型</param>
        /// <param name="extension">文件拓展名</param>
        /// <param name="filename">文件重命名</param>
        /// <returns></returns>
        async Task<PersonalFileInfo> SingleFile(string configId, Stream stream, string type, string extension, string filename)
        {
            if (!Operator.IsSuperAdmin && !AuthoritiesBusiness.CurrentAccountHasCFUC(configId))
                throw new MessageException("无权限, 禁止上传.");

            if (!CheckType(configId, type, extension))
                throw new MessageException("文件类型不合法, 禁止上传.");

            if (!Directory.Exists(BaseDir))
                Directory.CreateDirectory(BaseDir);

            var name = Guid.NewGuid().ToString();
            var path = Path.Combine(BaseDir, $"{name}{extension}");

            await Common.FileBusiness.Save(stream, path);

            if (!path.Exists())
                throw new MessageException("未上传任何文件.");

            var md5 = await Common.FileBusiness.GetMD5(path);

            var validation = PreUploadFile(configId, md5, filename, false, type);
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

            var file_extension = new Common_PersonalFileInfo
            {
                Name = filename,
                FileId = file.Id,
                State = PersonalFileInfoState.可用
            }.InitEntity();

            void handler()
            {
                Repository.Insert(file);

                Repository_FileExtension.Insert(file_extension);
            }

            (bool success, Exception ex) = Orm.RunTransaction(handler);

            if (!success)
                throw new MessageException("数据处理失败.", ex);

            return PersonalFileInfoBusiness.GetDetail(file_extension.Id);
        }

        /// <summary>
        /// 使用上传配置检查文件类型是否合法
        /// </summary>
        /// <param name="configId">上传配置Id</param>
        /// <param name="type">MIME类型</param>
        /// <param name="extension"></param>
        /// <returns></returns>
        bool CheckType(string configId, string type, string extension = null)
        {
            var config = Repository_FileUploadConfig.Where(o => o.Id == configId && o.Enable == true).ToOne(o => new { o.AllowedTypes, o.ProhibitedTypes });
            if (config == default)
                throw new MessageException("上传配置不存在或已失效");

            if (config.AllowedTypes.IsNullOrWhiteSpace() && config.ProhibitedTypes.IsNullOrWhiteSpace())
                return true;

            if (!config.AllowedTypes.IsNullOrWhiteSpace())
            {
                var flag = false;
                foreach (var item in config.AllowedTypes.Split(','))
                {
                    if ((!type.IsNullOrWhiteSpace() && new Regex(item.Replace("/*", "//*")).IsMatch(type)) || (!extension.IsNullOrWhiteSpace() && item[0] == '.' && item == extension))
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    return false;
                }
            }

            if (!config.ProhibitedTypes.IsNullOrWhiteSpace())
            {
                foreach (var item in config.ProhibitedTypes.Split(','))
                {
                    if ((!type.IsNullOrWhiteSpace() && new Regex(item.Replace("/*", "//*")).IsMatch(type)) || (!extension.IsNullOrWhiteSpace() && item[0] == '.' && item == extension))
                        return false;
                }
            }

            return true;
        }

        #endregion

        #region 外部接口

        #region 文件操作接口

        public PreUploadFileResponse PreUploadFile(string configId, string md5, string filename, bool section = false, string type = null, string extension = null, int? specs = null, int? total = null)
        {
            if (!Operator.IsSuperAdmin && !AuthoritiesBusiness.CurrentAccountHasCFUC(configId))
                throw new MessageException("无权限, 禁止上传.");

            if (!CheckType(configId, type, extension))
                throw new MessageException("文件类型不合法, 禁止上传.");

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
                                            && o.State == $"{PersonalFileInfoState.可用}"
                                            && o.CreatorId == Operator.AuthenticationInfo.Id)
                                        .ToOne(o => o.Id);

                if (file_extension_id == default)
                {
                    var file_extension = new Common_PersonalFileInfo
                    {
                        Name = filename,
                        FileId = state.Id,
                        State = PersonalFileInfoState.可用
                    }.InitEntity();

                    file_extension_id = file_extension.Id;

                    Repository_FileExtension.Insert(file_extension);
                }

                result.FileInfo = PersonalFileInfoBusiness.GetDetail(file_extension_id);
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

        public PersonalFileInfo UploadChunkFileFinished(string file_md5, int specs, int total, string type, string extension, string filename)
        {
            if (!Config.EnableUploadLargeFile)
                throw new MessageException("未启用大文件上传功能.");

            var file_state = GetFileState(file_md5, false);

            Common_File file = null;

            var file_extension = new Common_PersonalFileInfo
            {
                Name = filename,
                State = PersonalFileInfoState.可用
            };

            if (file_state.State == FileState.可用 || file_state.State == FileState.处理中)
            {
                file_extension.FileId = file_state.Id;

                file_extension.Id = Repository_FileExtension
                                        .Where(o =>
                                            o.FileId == file_state.Id
                                            && o.Name == filename
                                            && o.State == $"{PersonalFileInfoState.可用}"
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
            return PersonalFileInfoBusiness.GetDetail(file_extension.Id);
        }

        public async Task<PersonalFileInfo> SingleImageFromBase64(string base64, string filename)
        {
            if (!Directory.Exists(BaseDir))
                Directory.CreateDirectory(BaseDir);

            var name = Guid.NewGuid().ToString();
            var path = Path.Combine(BaseDir, $"{name}.bmp");

            var image = ImgHelper.GetImgFromBase64Url(base64);
            Common.FileBusiness.Save(image, path);
            var md5 = await Common.FileBusiness.GetMD5(path);

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

            var file_extension = new Common_PersonalFileInfo
            {
                Name = filename,
                FileId = file.Id,
                State = PersonalFileInfoState.可用
            }.InitEntity();

            void handler()
            {
                Repository.Insert(file);

                Repository_FileExtension.Insert(file_extension);
            }

            (bool success, Exception ex) = Orm.RunTransaction(handler);

            if (!success)
                throw new MessageException("数据处理失败.", ex);

            return PersonalFileInfoBusiness.GetDetail(file_extension.Id);
        }

        public async Task<PersonalFileInfo> SingleFileFromUrl(string url, string filename, bool download = false)
        {
            return await SingleFileFromUrl(null, url, filename, download);
        }

        public async Task<PersonalFileInfo> SingleFileFromUrl(string configId, string url, string filename, bool download = false)
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

                if (configId != null)
                {
                    if (!Operator.IsSuperAdmin && !AuthoritiesBusiness.CurrentAccountHasCFUC(configId))
                        throw new MessageException("无权限, 禁止上传.");

                    if (!CheckType(configId, contentType, extension))
                        throw new MessageException("文件类型不合法, 禁止上传.");
                }

                await Common.FileBusiness.Save(stream, path);

                var md5 = await Common.FileBusiness.GetMD5(path);

                if (!FileExists(md5, out Common_File _file))
                {
                    file = new Common_File
                    {
                        Name = name,
                        StorageType = StorageType.Path,
                        FileType = contentType.IsNullOrWhiteSpace() ? FileBusiness.GetFileTypeByExtension(extension) : FileBusiness.GetFileTypeByMIME(contentType),
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

            var file_extension = new Common_PersonalFileInfo
            {
                Name = filename,
                FileId = file.Id,
                State = PersonalFileInfoState.可用
            }.InitEntity();

            void handler()
            {
                Repository.Insert(file);

                Repository_FileExtension.Insert(file_extension);
            }

            (bool success, Exception ex) = Orm.RunTransaction(handler);

            if (!success)
                throw new MessageException("数据处理失败.", ex);

            return PersonalFileInfoBusiness.GetDetail(file_extension.Id);
        }

        public async Task<PersonalFileInfo> SingleFile(string configId, IFormFile file, string filename)
        {
            if (file == null)
                throw new MessageException("未上传任何文件.", filename);

            using var rs = file.OpenReadStream();
            return await SingleFile(configId, rs, file.ContentType, Path.GetExtension(file.FileName), filename);
        }

        public async Task<PersonalFileInfo> SingleFileByArrayBuffer(string configId, string type, string extension, string filename)
        {
            using var rs = HttpContextAccessor.HttpContext.Request.Body;

            return await SingleFile(configId, rs, type, extension, filename);
        }

        #endregion

        #endregion
    }
}

using AutoMapper;
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
using Microservice.Library.File.Model;
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
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using FileInfo = Model.Common.FileDTO.FileInfo;

namespace Business.Implementation.Common
{
    /// <summary>
    /// 文件信息业务类
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
            HttpContextAccessor = httpContextAccessor;
            HttpRequest = HttpContextAccessor?.HttpContext?.Request;
            HttpResponse = HttpContextAccessor?.HttpContext?.Response;

            FileStateDir = $"{Config.AbsoluteWWWRootDirectory}/filestate";
            PreviewDir = $"{Config.AbsoluteWWWRootDirectory}/filetypes";
        }

        #endregion

        #region 私有成员

        readonly IMapper Mapper;

        readonly IFreeSql Orm;

        readonly IBaseRepository<Common_File, string> Repository;

        readonly IHttpContextAccessor HttpContextAccessor;

        readonly HttpRequest HttpRequest;

        readonly HttpResponse HttpResponse;

        /// <summary>
        /// 文件状态图存储路径根目录绝对路径
        /// </summary>
        readonly string FileStateDir;

        /// <summary>
        /// 文件类型预览图存储路径根目录绝对路径
        /// </summary>
        readonly string PreviewDir;

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

        #endregion

        #region 外部接口

        public List<FileInfo> GetList(PaginationDTO pagination)
        {
            var entityList = Repository.Select
                        .GetPagination(pagination)
                        .ToList<Common_File, FileInfo>(typeof(FileInfo).GetNamesWithTagAndOther(true, "_List"));

            var result = Mapper.Map<List<FileInfo>>(entityList);

            return result;
        }

        public FileInfo GetDetail(string id)
        {
            var entity = Repository.GetAndCheckNull(id);

            var result = Mapper.Map<FileInfo>(entity);

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

        public void Delete(List<string> ids)
        {
            var files = Repository.Select
                .Where(c => ids.Contains(c.Id))
                .ToList(c => new
                {
                    c.Id,
                    c.Name,
                    c.StorageType,
                    c.Path,
                    c.Extension
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

                void handler()
                {
                    if (Repository.UpdateDiy
                        .Where(o => o.Id == file.Id)
                        .Set(o => o.State, FileState.已删除)
                        .ExecuteAffrows() <= 0)
                        throw new MessageException("更新文件信息失败.", new { file.Id });

                    if (Orm.GetRepository<Common_PersonalFileInfo>().UpdateDiy
                        .Where(o => o.FileId == file.Id)
                        .Set(o => o.State, PersonalFileInfoState.不可用)
                        .ExecuteAffrows() < 0)
                        throw new MessageException("更新个人文件信息失败.", new { file.Id });
                }

                (bool success, Exception ex) = Orm.RunTransaction(handler);

                if (!success)
                    throw new MessageException("文件信息更新失败.", ex);
            }
        }

        public async Task Preview(string id, int? width = null, int? height = null, TimeSpan? time = null)
        {
            var file = Repository.Get(id);
            if (file == null)
            {
                Response(HttpResponse, StatusCodes.Status404NotFound, "文件不存在或已被删除.");
                return;
            }

            if (!await CheckFileStateResponseWhenError(file.State))
                return;

            if (file.StorageType == StorageType.Uri)
            {
                HttpResponse.Redirect(file.Path);
                return;
            }
            else if (file.StorageType != StorageType.Path)
            {
                Response(HttpResponse, StatusCodes.Status412PreconditionFailed, "此文件不支持预览.");
                return;
            }

            if (!FileHelper.Exists(file.Path))
            {
                Response(HttpResponse, StatusCodes.Status404NotFound, "文件不存在或已被删除.");
                return;
            }
            //throw new MessageException("文件已被删除.", file.Path);

            width ??= 500;
            height ??= 500;

            if (file.FileType == FileType.图片)
            {
                var imagePath = $"{(file.Extension.IsNullOrWhiteSpace() ? file.Path : file.Path.Replace(file.Extension, ""))}-Screenshot/{width}x{height}{file.Extension}";

                if (!imagePath.Exists())
                {
                    var dir = imagePath[..imagePath.LastIndexOf('/')];
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    try
                    {
                        using var fs = new FileStream(file.Path, FileMode.Open, FileAccess.Read);
                        var image = new Image(fs);

                        using var newFS = File.Create(imagePath);
                        image.Resize(width.Value, height.Value)
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

                HttpResponse.ContentType = file.ContentType.IsNullOrWhiteSpace() ? "applicatoin/octet-stream" : file.ContentType;
                await ResponseFile(HttpRequest, HttpResponse, imagePath);
            }
            else if (file.FileType == FileType.视频)
            {
                time ??= TimeSpan.FromSeconds(0.001);
                var imagePath = $"{(file.Extension.IsNullOrWhiteSpace() ? file.Path : file.Path.Replace(file.Extension, ""))}-Screenshot/{width}x{height}-{time.Value.ToString("c").Replace(':', '：')}.jpg";

                if (!File.Exists(imagePath))
                {
                    try
                    {
                        var ffmpegPath = Config.GetFileAbsolutePath("ffmpeg");

                        file.Path.Screenshot(ffmpegPath, imagePath, time.Value, 31, width, height);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(
                            NLog.LogLevel.Warn,
                            LogType.警告信息,
                            "视频文件截图失败.",
                            $"File: {file.Path}, Time: {time.Value:c}, quality: 31, width: {width}, height: {height}.",
                            ex);

                        HttpResponse.ContentType = "image/jpg";
                        await ResponseFile(HttpRequest, HttpResponse, GetFileTypeImage(file.Extension));
                        return;
                    }
                }

                HttpResponse.ContentType = "image/jpg";
                await ResponseFile(HttpRequest, HttpResponse, imagePath);
            }
            else
            {
                HttpResponse.ContentType = "image/jpg";
                await ResponseFile(HttpRequest, HttpResponse, GetFileTypeImage(file.Extension));
            }

            //throw new MessageException("此文件不支持预览.");
        }

        public async Task Browse(string id)
        {
            var file = Repository.Get(id);
            if (file == null)
            {
                Response(HttpResponse, StatusCodes.Status404NotFound, "文件不存在或已被删除.");
                return;
            }

            if (!await CheckFileStateResponseWhenError(file.State))
                return;

            if (file.StorageType == StorageType.Uri)
            {
                HttpResponse.Redirect(file.Path);
                return;
            }
            else if (file.StorageType != StorageType.Path)
            {
                Response(HttpResponse, StatusCodes.Status412PreconditionFailed, "此文件不支持浏览.");
                return;
            }
            else if (file.FileType != FileType.图片 && file.FileType != FileType.视频 && file.FileType != FileType.文本文件 && file.ContentType != "application/pdf")
            {
                Response(HttpResponse, StatusCodes.Status412PreconditionFailed, "此文件不支持浏览.");
                return;
            }

            HttpResponse.ContentType = file.ContentType.IsNullOrWhiteSpace() ? "applicatoin/octet-stream" : file.ContentType;

            await ResponseFile(HttpRequest, HttpResponse, file.Path, file.Bytes);
        }

        public async Task Download(string id, string rename = null)
        {
            var file = Repository.Get(id);
            if (file == null)
            {
                Response(HttpResponse, StatusCodes.Status404NotFound, "文件不存在或已被删除.");
                return;
            }

            if (!await CheckFileStateResponseWhenError(file.State))
                return;

            if (file.StorageType == StorageType.Uri)
            {
                HttpResponse.Redirect(file.Path);
                return;
            }
            else if (file.StorageType != StorageType.Path)
            {
                Response(HttpResponse, StatusCodes.Status412PreconditionFailed, "此文件不支持下载.");
                return;
            }

            HttpResponse.ContentType = file.ContentType.IsNullOrWhiteSpace() ? "applicatoin/octet-stream" : file.ContentType;
            HttpResponse.Headers.Add("Content-Disposition", $"attachment; filename=\"{UrlEncoder.Default.Encode($"{(rename.IsNullOrWhiteSpace() ? file.Name : rename)}{file.Extension}")}\"");

            await ResponseFile(HttpRequest, HttpResponse, file.Path, file.Bytes);
        }

        public async Task Download(string id, string dirPath, string rename = null)
        {
            var file = Repository.Get(id);
            if (file == null)
            {
                throw new MessageException("文件不存在或已被删除.");
            }

            if (!await CheckFileStateResponseWhenError(file.State))
                return;

            var path = Path.Combine(dirPath, $"{(rename.IsNullOrWhiteSpace() ? file.Name : rename)}{file.Extension}");

            if (file.StorageType == StorageType.Uri)
            {
                using var client = new HttpClient();
                using var dsfs = await client.GetStreamAsync(file.Path);
                await Copy(dsfs);
                return;
            }
            else if (file.StorageType != StorageType.Path)
            {
                throw new MessageException("此文件不支持下载.");
            }

            using var sfs = new FileStream(file.Path, FileMode.Open);
            await Copy(sfs);

            async Task Copy(Stream sfs)
            {
                using var dfs = new FileStream(path, FileMode.CreateNew);
                await sfs.CopyToAsync(dfs);
            }
        }

        #region 拓展功能

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
            HttpResponse.ContentType = "image/jpg";
            await ResponseFile(HttpRequest, HttpResponse, GetFileTypeImage(extension.ToLower()));
        }

        public string GetFileSize(string length)
        {
            if (!long.TryParse(length, out long value))
                throw new MessageException("length值格式有误.");
            return value.GetFileSize();
        }

        public VideoInfo GetVideoInfo(string id, bool format = true, bool streams = false, bool chapters = false, bool programs = false, bool version = false)
        {
            var file = Repository.GetAndCheckNull(id, "文件不存在或已被删除.");

            if (file.FileType != FileType.视频)
                throw new MessageException("文件不是视频文件.");

            if (file.State != FileState.可用)
                throw new MessageException("文件不可用.");

            try
            {
                var ffprobePath = Config.GetFileAbsolutePath("ffprobe");

                return file.Path.GetVideoInfo(ffprobePath, format, streams, chapters, programs, version);
            }
            catch (Exception ex)
            {
                throw new MessageException("获取视频信息失败.", new { file.Path, format, streams, chapters, programs, version }, ex);
            }
        }

        public List<LibraryInfo> GetLibraryInfo()
        {
            return Repository.Select.Where(o => o.State == $"{FileState.可用}")
                .GroupBy(o => o.FileType)
                .ToList(g => new LibraryInfo
                {
                    FileType = g.Key,
                    Total = g.Count(),
                    _Bytes = (long)g.Sum(g.Value.Bytes)
                })
                .Select(o =>
                {
                    o.Bytes = o._Bytes.ToString();
                    o.Size = o._Bytes.GetFileSize();
                    return o;
                })
                .ToList();
        }

        public List<string> GetFileTypes()
        {
            var result = typeof(FileType)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
                .Select(o => o.GetValue(null).ToString())
                .ToList();
            return result;
        }

        public List<string> GetStorageTypes()
        {
            var result = typeof(StorageType)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
                .Select(o => o.GetValue(null).ToString())
                .ToList();
            return result;
        }

        public List<string> GetFileStates()
        {
            var result = typeof(FileState)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
                .Select(o => o.GetValue(null).ToString())
                .ToList();
            return result;
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
            {
                var end = range[1].ToLong();
                if (end <= start)
                    end = bytes;

                count = end - start;
            }

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

        /// <summary>
        /// 遍历文件夹
        /// </summary>
        /// <param name="directory">文件夹</param>
        /// <param name="fileCount">文件数量</param>
        /// <param name="fileLength">文件总字节数</param>
        /// <param name="delete">删除文件夹</param>
        public static void ErgodicDirectorie(DirectoryInfo directory, out int fileCount, out long fileLength, bool delete = false)
        {
            fileCount = 0;
            fileLength = 0;

            if (!directory.Exists)
                return;

            var directories = directory.GetDirectories();
            foreach (var deep_directory in directories)
            {
                ErgodicDirectorie(deep_directory, out int deep_fileCount, out long deep_fileLength, delete);
                fileCount += deep_fileCount;
                fileLength += deep_fileLength;

                if (delete)
                    deep_directory.Delete();
            }

            var files = directory.GetFiles();

            foreach (var file in files)
            {
                fileCount++;
                fileLength += file.Length;

                if (delete)
                    file.Delete();
            }
        }

        #endregion

        #endregion
    }
}

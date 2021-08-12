using Api.Controllers.Utils;
using Business.Interface.Common;
using Business.Utils.Authorization;
using Microservice.Library.Extension;
using Microservice.Library.OpenApi.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.Common;
using Model.Common.FileDTO;
using Model.Utils.Pagination;
using Model.Utils.Result;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Api.Controllers
{
    /// <summary>
    /// 文件处理接口
    /// </summary>
    [Route("/file")]
    [SampleAuthorize(nameof(ApiAuthorizeRequirement))]
    [SwaggerTag("文件处理接口")]
    public class FileController : BaseController
    {
        #region DI

        public FileController(IFileBusiness fileBusiness)
        {
            FileBusiness = fileBusiness;
        }

        readonly IFileBusiness FileBusiness;

        #endregion

        #region 数据接口

        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="pagination">分页设置</param>
        /// <returns></returns>
        [HttpPost("list")]
        [Consumes("application/json", "application/x-www-form-urlencoded")]
        [Produces("application/json")]
        [SwaggerResponse((int)HttpStatusCode.OK, "列表数据", typeof(FileInfo))]
        public async Task<object> GetList([FromBody] PaginationDTO pagination)
        {
            return await Task.FromResult(OpenApiJsonContent(FileBusiness.GetList(pagination), pagination));
        }

        /// <summary>
        /// 获取详情数据
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        [HttpPost("detail-data/{id}")]
        [Consumes("application/json", "application/x-www-form-urlencoded")]
        [Produces("application/json")]
        [SwaggerResponse((int)HttpStatusCode.OK, "详情数据", typeof(FileInfo))]
        public async Task<object> GetDetail(string id)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(FileBusiness.GetDetail(id))));
        }

        /// <summary>
        /// 获取详情数据集合
        /// </summary>
        /// <param name="ids">id逗号拼接</param>
        /// <returns></returns>
        [HttpGet("detail-list/{ids}")]
        [Consumes("application/json", "application/x-www-form-urlencoded")]
        [Produces("application/json")]
        [SwaggerResponse((int)HttpStatusCode.OK, "详情数据", typeof(FileInfo))]
        public async Task<object> GetDetails(string ids)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(FileBusiness.GetDetails(ids))));
        }

        /// <summary>
        /// 获取详情数据集合
        /// </summary>
        /// <param name="ids">id集合</param>
        /// <returns></returns>
        [HttpPost("detail-list")]
        [Consumes("application/json", "application/x-www-form-urlencoded")]
        [Produces("application/json")]
        [SwaggerResponse((int)HttpStatusCode.OK, "详情数据", typeof(FileInfo))]
        public async Task<object> GetDetails(List<string> ids)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(FileBusiness.GetDetails(ids))));
        }

        /// <summary>
        /// 文件重命名
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="filename">文件名</param>
        /// <returns></returns>
        [HttpGet("rename/{id}")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<object> Rename(string id, string filename)
        {
            FileBusiness.Rename(id, filename);
            return await Task.FromResult(Success());
        }

        #endregion

        #region 文件操作接口

        /// <summary>
        /// 获取文件类型
        /// </summary>
        /// <param name="extension">文件拓展名</param>
        /// <returns></returns>
        [HttpGet("file-type-by-extension/{extension}")]
        [SwaggerResponse((int)HttpStatusCode.OK, "文件类型", typeof(string))]
        [AllowAnonymous]
        public async Task<object> GetFileTypeByExtension(string extension)
        {
            return await Task.FromResult(ResponseDataFactory.Success<string>(FileBusiness.GetFileTypeByExtension(extension)));
        }

        /// <summary>
        /// 获取文件类型
        /// </summary>
        /// <param name="mimetype">MIME类型</param>
        /// <returns></returns>
        [HttpGet("file-type-by-mimetype")]
        [SwaggerResponse((int)HttpStatusCode.OK, "文件类型", typeof(string))]
        [AllowAnonymous]
        public async Task<object> GetFileTypeByMIME(string mimetype)
        {
            return await Task.FromResult(ResponseDataFactory.Success<string>(FileBusiness.GetFileTypeByMIME(mimetype)));
        }

        /// <summary>
        /// 获取文件类型预览图链接地址
        /// </summary>
        /// <param name="extension">文件拓展名</param>
        /// <returns></returns>
        [HttpGet("file-type-image/{extension}")]
        [AllowAnonymous]
        public async Task GetFileTypeImageUrl(string extension)
        {
            await FileBusiness.GetFileTypeImageUrl(extension);
        }

        /// <summary>
        /// 获取文件大小
        /// </summary>
        /// <param name="length">文件字节数</param>
        /// <returns></returns>
        [HttpGet("file-size/{length}")]
        [SwaggerResponse((int)HttpStatusCode.OK, "文件大小", typeof(string))]
        [AllowAnonymous]
        public async Task<object> GetFileSize(string length)
        {
            return await Task.FromResult(ResponseDataFactory.Success<string>(FileBusiness.GetFileSize(length)));
        }

        /// <summary>
        /// 预备上传文件
        /// </summary>
        /// <param name="md5">文件MD5值</param>
        /// <param name="filename">文件重命名</param>
        /// <param name="section">是否分片处理（默认否）</param>
        /// <param name="type">文件类型（单文件上传时忽略此参数）</param>
        /// <param name="extension">文件拓展名（单文件上传时忽略此参数）</param>
        /// <param name="specs">分片文件规格（单文件上传时忽略此参数）</param>
        /// <param name="total">分片文件总数（单文件上传时忽略此参数）</param>
        /// <returns></returns>
        [HttpGet("pre-upload-file/{md5}")]
        [SwaggerResponse((int)HttpStatusCode.OK, "输出信息", typeof(PreUploadFileResponse))]
        public async Task<object> PreUploadFile(string md5, string filename, bool section = false, string type = null, string extension = null, int? specs = null, int? total = null)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(FileBusiness.PreUploadFile(md5, filename, section, type, extension, specs, total))));
        }

        /// <summary>
        /// 预备上传分片文件
        /// </summary>
        /// <param name="file_md5">文件MD5值</param>
        /// <param name="md5">分片文件MD5值</param>
        /// <param name="index">分片文件索引</param>
        /// <param name="specs">分片文件规格</param>
        /// <param name="forced">强制上传</param>
        /// <returns></returns>
        [HttpGet("pre-upload-chunkfile/{file_md5}/{md5}/{index}/{specs}")]
        [SwaggerResponse((int)HttpStatusCode.OK, "输出信息", typeof(PreUploadChunkFileResponse))]
        public async Task<object> PreUploadChunkFile(string file_md5, string md5, int index, int specs, bool forced = false)
        {
            return await Task.FromResult(ResponseDataFactory.Success(FileBusiness.PreUploadChunkFile(file_md5, md5, index, specs, forced)));
        }

        /// <summary>
        /// 上传单个分片文件
        /// </summary>
        /// <param name="key">上传标识</param>
        /// <param name="md5">分片文件MD5值</param>
        /// <param name="file">分片文件</param>
        /// <returns></returns>
        [Consumes("multipart/form-data")]
        [HttpPost("upload-single-chunkfile/{key}/{md5}")]
        public async Task<object> SingleChunkFile(string key, string md5, IFormFile file)
        {
            await FileBusiness.SingleChunkFile(key, md5, file);
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 上传单个分片文件
        /// </summary>
        /// <param name="key">上传标识</param>
        /// <param name="md5">分片文件MD5值</param>
        /// <returns></returns>
        [Consumes("applicatoin/octet-stream")]
        [HttpPost("upload-single-chunkfile-arraybuffer/{key}/{md5}")]
        public async Task<object> SingleChunkFileByArrayBuffer(string key, string md5)
        {
            await FileBusiness.SingleChunkFileByArrayBuffer(key, md5);
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 分片文件全部上传完毕
        /// </summary>
        /// <param name="file_md5">文件MD5值</param>
        /// <param name="specs">分片文件规格</param>
        /// <param name="total">分片文件总数</param>
        /// <param name="type">文件类型</param>
        /// <param name="extension">文件拓展名</param>
        /// <param name="filename">文件名</param>
        /// <returns></returns>
        [HttpGet("upload-chunkfile-finished/{file_md5}/{specs}/{total}")]
        [SwaggerResponse((int)HttpStatusCode.OK, "文件信息", typeof(FileInfo))]
        public async Task<object> UploadChunkFileFinished(string file_md5, int specs, int total, string type, string extension, string filename)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(FileBusiness.UploadChunkFileFinished(file_md5, specs, total, type, extension, filename))));
        }

        /// <summary>
        /// 通过Base64字符串上传单个图片
        /// </summary>
        /// <param name="base64">Base64字符串</param>
        /// <param name="filename">文件名</param>
        /// <returns></returns>
        [HttpPost("upload-single-image-base64")]
        [SwaggerResponse((int)HttpStatusCode.OK, "文件信息", typeof(FileInfo))]
        public async Task<object> SingleImageFromBase64([FromBody] string base64, string filename)
        {
            return OpenApiJsonContent(ResponseDataFactory.Success(await FileBusiness.SingleImageFromBase64(base64, filename)));
        }

        /// <summary>
        /// 通过外链上传单个文件
        /// </summary>
        /// <param name="url">外链地址</param>
        /// <param name="filename">文件名</param>
        /// <param name="download">是否下载资源</param>
        /// <returns></returns>
        [HttpPost("upload-single-file-url")]
        [SwaggerResponse((int)HttpStatusCode.OK, "文件信息", typeof(FileInfo))]
        public async Task<object> SingleFileFromUrl(string url, string filename, bool download = false)
        {
            return OpenApiJsonContent(ResponseDataFactory.Success(await FileBusiness.SingleFileFromUrl(url, filename, download)));
        }

        /// <summary>
        /// 上传单个文件
        /// </summary>
        /// <param name="file">分片文件</param>
        /// <param name="filename">文件名</param>
        /// <returns></returns>
        [Consumes("multipart/form-data")]
        [HttpPost("upload-single-file")]
        [SwaggerResponse((int)HttpStatusCode.OK, "文件信息", typeof(FileInfo))]
        public async Task<object> SingleFile(IFormFile file, string filename)
        {
            return OpenApiJsonContent(ResponseDataFactory.Success(await FileBusiness.SingleFile(file, filename)));
        }

        /// <summary>
        /// 上传单个文件
        /// </summary>
        /// <param name="type">上传标识</param>
        /// <param name="extension">文件拓展名</param>
        /// <param name="filename">文件名</param>
        /// <returns></returns>
        [Consumes("applicatoin/octet-stream")]
        [HttpPost("upload-single-file-arraybuffer")]
        [SwaggerResponse((int)HttpStatusCode.OK, "文件信息", typeof(FileInfo))]
        public async Task<object> SingleFileByArrayBuffer(string type, string extension, string filename)
        {
            return OpenApiJsonContent(ResponseDataFactory.Success(await FileBusiness.SingleFileByArrayBuffer(type, extension, filename)));
        }

        /// <summary>
        /// 预览
        /// </summary>
        /// <remarks>用于查看文件缩略图或视频截图</remarks>
        /// <param name="id">文件Id</param>
        /// <param name="width">指定宽度</param>
        /// <param name="height">指定高度</param>
        /// <param name="time">视频的时间轴位置（默认值: 00:00:00.001）</param>
        /// <returns></returns>
        [HttpGet("preview/{id}")]
        public async Task Preview(string id, int width = 500, int height = 500, TimeSpan? time = null)
        {
            await FileBusiness.Preview(id, width, height, time);
        }

        /// <summary>
        /// 浏览
        /// </summary>
        /// <param name="id">文件Id</param>
        /// <returns></returns>
        [HttpGet("browse/{id}")]
        public async Task Browse(string id)
        {
            await FileBusiness.Browse(id);
        }

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="id">文件Id</param>
        /// <returns></returns>
        [HttpGet("download/{id}")]
        public async Task Download(string id)
        {
            await FileBusiness.Download(id);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids">Id集合</param>
        /// <returns></returns>
        [HttpPost("delete")]
        [Consumes("application/json", "application/x-www-form-urlencoded")]
        [Produces("application/json")]
        public async Task<object> Delete(IEnumerable<string> ids)
        {
            FileBusiness.Delete(ids?.ToList());
            return await Task.FromResult(Success());
        }

        #endregion        
    }
}

using Api.Controllers.Utils;
using Business.Interface.Common;
using Business.Utils.Authorization;
using Microservice.Library.Extension;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    [Consumes("multipart/form-data")]
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
            return await Task.FromResult(OpenApiJsonContent(AjaxResultFactory.Success(FileBusiness.GetDetail(id))));
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
            return await Task.FromResult(OpenApiJsonContent(AjaxResultFactory.Success(FileBusiness.GetDetails(ids))));
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
            return await Task.FromResult(OpenApiJsonContent(AjaxResultFactory.Success(FileBusiness.GetDetails(ids))));
        }

        #endregion

        #region 文件操作接口

        /// <summary>
        /// 文件MD5值校验
        /// </summary>
        /// <param name="md5"></param>
        /// <param name="filename">文件重命名</param>
        /// <returns></returns>
        [HttpGet("validation-file-md5/{md5}")]
        [SwaggerResponse((int)HttpStatusCode.OK, "输出信息", typeof(ValidationMD5Response))]
        public async Task<object> ValidationFileMD5(string md5, string filename = null)
        {
            return await Task.FromResult(OpenApiJsonContent(AjaxResultFactory.Success(FileBusiness.ValidationFileMD5(md5, filename))));
        }

        /// <summary>
        /// 分片MD5值校验
        /// </summary>
        /// <param name="file_md5">文件MD5值</param>
        /// <param name="md5">分片文件MD5值</param>
        /// <param name="index">分片文件索引</param>
        /// <param name="specs">分片文件规格</param>
        /// <returns></returns>
        [HttpGet("pre-upload-chunkfile/{file_md5}/{md5}/{index}/{specs}")]
        [SwaggerResponse((int)HttpStatusCode.OK, "输出信息", typeof(PreUploadChunkFileResponse))]
        public async Task<object> PreUploadChunkFile(string file_md5, string md5, int index, int specs)
        {
            return await Task.FromResult(AjaxResultFactory.Success(FileBusiness.PreUploadChunkFile(file_md5, md5, index, specs)));
        }

        /// <summary>
        /// 单分片文件上传
        /// </summary>
        /// <param name="key">上传标识</param>
        /// <param name="md5">分片文件MD5值</param>
        /// <param name="file">分片文件</param>
        /// <returns></returns>
        [HttpPost("upload-single-chunkfile/{key}/{md5}")]
        public async Task<object> SingleChunkFile(string key, string md5, [FromForm] IFormFile file)
        {
            await FileBusiness.SingleChunkFile(key, md5, file);
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 分片文件全部上传完毕
        /// </summary>
        /// <param name="file_md5">文件MD5值</param>
        /// <param name="specs">分片文件规格</param>
        /// <param name="total">分片文件总数</param>
        /// <param name="filename">文件重命名</param>
        /// <returns></returns>
        [HttpGet("upload-chunkfile-finished/{file_md5}/{specs}/{total}")]
        [SwaggerResponse((int)HttpStatusCode.OK, "文件信息", typeof(FileInfo))]
        public async Task<object> UploadChunkFileFinished(string file_md5, int specs, int total, string filename = null)
        {
            return await Task.FromResult(OpenApiJsonContent(AjaxResultFactory.Success(FileBusiness.UploadChunkFileFinished(file_md5, specs, total, filename))));
        }

        /// <summary>
        /// 通过外链上传单个图片
        /// </summary>
        /// <param name="url">外链地址</param>
        /// <param name="download">是否下载资源</param>
        /// <param name="filename">文件重命名</param>
        /// <returns></returns>
        [HttpPost("upload-single-image-url")]
        [SwaggerResponse((int)HttpStatusCode.OK, "文件信息", typeof(FileInfo))]
        public async Task<object> SingleImageFromUrl([FromBody] string url, bool download = false, string filename = null)
        {
            return await Task.FromResult(OpenApiJsonContent(AjaxResultFactory.Success(FileBusiness.SingleImageFromUrl(url, download, filename))));
        }

        /// <summary>
        /// 通过外链上传单个文件
        /// </summary>
        /// <param name="url">外链地址</param>
        /// <param name="download">是否下载资源</param>
        /// <param name="filename">文件重命名</param>
        /// <returns></returns>
        [HttpPost("upload-single-file-url")]
        [SwaggerResponse((int)HttpStatusCode.OK, "文件信息", typeof(FileInfo))]
        public async Task<object> SingleFileFromUrl([FromBody] string url, bool download = false, string filename = null)
        {
            return await Task.FromResult(OpenApiJsonContent(AjaxResultFactory.Success(FileBusiness.SingleFileFromUrl(url, download, filename))));
        }

        /// <summary>
        /// 通过Base64字符串上传单个图片
        /// </summary>
        /// <param name="base64">Base64字符串</param>
        /// <param name="filename">文件重命名</param>
        /// <returns></returns>
        [HttpPost("upload-single-image-base64")]
        [SwaggerResponse((int)HttpStatusCode.OK, "文件信息", typeof(FileInfo))]
        public async Task<object> SingleImageFromBase64([FromBody] string base64, string filename = null)
        {
            return await Task.FromResult(OpenApiJsonContent(AjaxResultFactory.Success(FileBusiness.SingleImageFromBase64(base64, filename))));
        }

        /// <summary>
        /// 上传单个文件
        /// </summary>
        /// <param name="file">分片文件</param>
        /// <param name="filename">文件重命名</param>
        /// <returns></returns>
        [HttpPost("upload-single-file")]
        [SwaggerResponse((int)HttpStatusCode.OK, "文件信息", typeof(FileInfo))]
        public async Task<object> SingleFile([FromForm] IFormFile file, string filename = null)
        {
            return await Task.FromResult(OpenApiJsonContent(AjaxResultFactory.Success(FileBusiness.SingleFile(file, filename))));
        }

        ///// <summary>
        ///// 单图上传
        ///// </summary>
        ///// <param name="option">选项</param>
        ///// <returns></returns>
        //[HttpPost("upload-single-image")]
        //[SwaggerResponse((int)HttpStatusCode.OK, "文件信息", typeof(FileInfo))]
        //public async Task<object> UploadSingleImage(ImageUploadParams option)
        //{
        //    return await Task.FromResult(OpenApiJsonContent(AjaxResultFactory.Success(FileBusiness.SingleImage(option))));
        //}

        ///// <summary>
        ///// 单文件上传
        ///// </summary>
        ///// <param name="option">选项</param>
        ///// <returns></returns>
        //[HttpPost("upload-single-file")]
        //[SwaggerResponse((int)HttpStatusCode.OK, "文件信息", typeof(FileInfo))]
        //public async Task<object> UploadSingleFile(FileUploadParams option)
        //{
        //    return await Task.FromResult(OpenApiJsonContent(AjaxResultFactory.Success(FileBusiness.SingleFile(option))));
        //}

        /// <summary>
        /// 预览
        /// </summary>
        /// <remarks>用于查看文件缩略图或视频截图</remarks>
        /// <param name="id">文件Id</param>
        /// <param name="width">指定宽度</param>
        /// <param name="height">指定高度</param>
        /// <param name="time">视频的时间轴位置</param>
        /// <returns></returns>
        [HttpGet("preview/{id}")]
        public void Preview(string id, int width = 300, int height = 300, TimeSpan? time = null)
        {
            FileBusiness.Preview(id, width, height, time);
        }

        /// <summary>
        /// 浏览
        /// </summary>
        /// <param name="id">文件Id</param>
        /// <returns></returns>
        [HttpGet("browse/{id}")]
        public void Browse(string id)
        {
            FileBusiness.Browse(id);
        }

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="id">文件Id</param>
        /// <returns></returns>
        [HttpGet("download/{id}")]
        public void Download(string id)
        {
            FileBusiness.Download(id);
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

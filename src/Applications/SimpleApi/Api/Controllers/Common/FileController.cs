using Api.Controllers.Utils;
using Business.Interface.Common;
using Business.Utils.Authorization;
using Microservice.Library.Extension;
using Microservice.Library.File.Model;
using Microsoft.AspNetCore.Authorization;
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
    /// 文件信息接口
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "文件管理")]
    [Route("/file")]
    [SampleAuthorize(nameof(ApiAuthorizeRequirement))]
    [SwaggerTag("文件信息接口")]
    public class FileController : BaseController
    {
        #region DI

        public FileController(IFileBusiness fileBusiness)
        {
            FileBusiness = fileBusiness;
        }

        readonly IFileBusiness FileBusiness;

        #endregion

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
        [HttpGet("detail-data/{id}")]
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
        /// 预览
        /// </summary>
        /// <remarks>用于查看文件缩略图或视频截图</remarks>
        /// <param name="id">文件Id</param>
        /// <param name="width">指定宽度（默认值: 500）</param>
        /// <param name="height">指定高度（默认值: 500）</param>
        /// <param name="time">视频的时间轴位置（默认值: 00:00:00.001）</param>
        /// <returns></returns>
        [HttpGet("preview/{id}")]
        public async Task Preview(string id, int? width = null, int? height = null, TimeSpan? time = null)
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

        #region 拓展功能

        /// <summary>
        /// 获取文件类型
        /// </summary>
        /// <param name="extension">文件拓展名</param>
        /// <returns></returns>
        [HttpGet("type-by-extension/{extension}")]
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
        [HttpGet("type-by-mimetype")]
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
        [HttpGet("type-image/{extension}")]
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
        [HttpGet("size/{length}")]
        [SwaggerResponse((int)HttpStatusCode.OK, "文件大小", typeof(string))]
        [AllowAnonymous]
        public async Task<object> GetFileSize(string length)
        {
            return await Task.FromResult(ResponseDataFactory.Success<string>(FileBusiness.GetFileSize(length)));
        }

        /// <summary>
        /// 获取视频信息
        /// </summary>
        /// <param name="id">文件Id</param>
        /// <param name="format">获取有关输入多媒体流的容器格式的信息</param>
        /// <param name="streams">获取有关输入多媒体流中包含的每个媒体流的信息</param>
        /// <param name="chapters">获取有关以该格式存储的章节的信息</param>
        /// <param name="programs">获取有关程序及其输入多媒体流中包含的流的信息</param>
        /// <param name="version">获取与程序版本有关的信息、获取与库版本有关的信息、获取与程序和库版本有关的信息</param>
        /// <returns></returns>
        [HttpGet("video-info/{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, "视频信息", typeof(VideoInfo))]
        [AllowAnonymous]
        public async Task<object> GetVideoInfo(string id, bool format = true, bool streams = false, bool chapters = false, bool programs = false, bool version = false)
        {
            return ResponseDataFactory.Success(await FileBusiness.GetVideoInfo(id, format, streams, chapters, programs, version));
        }

        /// <summary>
        /// 获取文件库信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("library-info")]
        [SwaggerResponse((int)HttpStatusCode.OK, "文件库信息", typeof(LibraryInfo))]
        [AllowAnonymous]
        public async Task<object> GetLibraryInfo()
        {
            return await Task.FromResult(ResponseDataFactory.Success(FileBusiness.GetLibraryInfo()));
        }

        /// <summary>
        /// 获取所有文件类型
        /// </summary>
        /// <returns></returns>
        [HttpGet("filetypes")]
        public async Task<object> GetFileTypes()
        {
            return await Task.FromResult(Success(FileBusiness.GetFileTypes()));
        }

        /// <summary>
        /// 获取所有文件存储类型
        /// </summary>
        /// <returns></returns>
        [HttpGet("storagetypes")]
        public async Task<object> GetStorageTypes()
        {
            return await Task.FromResult(Success(FileBusiness.GetStorageTypes()));
        }

        /// <summary>
        /// 获取所有文件状态
        /// </summary>
        /// <returns></returns>
        [HttpGet("filestates")]
        public async Task<object> GetFileStates()
        {
            return await Task.FromResult(Success(FileBusiness.GetFileStates()));
        }

        #endregion
    }
}

using Api.Controllers.Utils;
using Business.Interface.Common;
using Business.Utils.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.Common.FileDTO;
using Model.Common.FileUploadDTO;
using Model.Common.PersonalFileInfoDTO;
using Model.Utils.Result;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Threading.Tasks;

namespace Api.Controllers
{
    /// <summary>
    /// 文件上传接口
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "文件管理")]
    [Route("/file-upload")]
    [SampleAuthorize(nameof(ApiWithoutPermissionRequirement))]
    [SwaggerTag("文件上传接口")]
    public class FileUploadController : BaseController
    {
        #region DI

        public FileUploadController(IFileUploadBusiness fileUploadBusiness)
        {
            FileUploadBusiness = fileUploadBusiness;
        }

        readonly IFileUploadBusiness FileUploadBusiness;

        #endregion

        /// <summary>
        /// 预备上传文件
        /// </summary>
        /// <param name="configId">上传配置Id</param>
        /// <param name="md5">文件MD5值</param>
        /// <param name="filename">文件重命名</param>
        /// <param name="section">是否分片处理（默认否）</param>
        /// <param name="type">文件类型（单文件上传时忽略此参数）</param>
        /// <param name="extension">文件拓展名（单文件上传时忽略此参数）</param>
        /// <param name="specs">分片文件规格（单文件上传时忽略此参数）</param>
        /// <param name="total">分片文件总数（单文件上传时忽略此参数）</param>
        /// <returns></returns>
        [HttpGet("pre-file/{configId}/{md5}")]
        [SwaggerResponse((int)HttpStatusCode.OK, "输出信息", typeof(PreUploadFileResponse))]
        public async Task<object> PreUploadFile(string configId, string md5, string filename, bool section = false, string type = null, string extension = null, int? specs = null, int? total = null)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(FileUploadBusiness.PreUploadFile(configId, md5, filename, section, type, extension, specs, total))));
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
        [HttpGet("pre-chunkfile/{file_md5}/{md5}/{index}/{specs}")]
        [SwaggerResponse((int)HttpStatusCode.OK, "输出信息", typeof(PreUploadChunkFileResponse))]
        public async Task<object> PreUploadChunkFile(string file_md5, string md5, int index, int specs, bool forced = false)
        {
            return await Task.FromResult(ResponseDataFactory.Success(FileUploadBusiness.PreUploadChunkFile(file_md5, md5, index, specs, forced)));
        }

        /// <summary>
        /// 上传单个分片文件
        /// </summary>
        /// <param name="key">上传标识</param>
        /// <param name="md5">分片文件MD5值</param>
        /// <param name="file">分片文件</param>
        /// <returns></returns>
        [Consumes("multipart/form-data")]
        [HttpPost("single-chunkfile/{key}/{md5}")]
        public async Task<object> SingleChunkFile(string key, string md5, IFormFile file)
        {
            await FileUploadBusiness.SingleChunkFile(key, md5, file);
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 上传单个分片文件
        /// </summary>
        /// <param name="key">上传标识</param>
        /// <param name="md5">分片文件MD5值</param>
        /// <returns></returns>
        [Consumes("application/octet-stream")]
        [HttpPost("single-chunkfile-arraybuffer/{key}/{md5}")]
        public async Task<object> SingleChunkFileByArrayBuffer(string key, string md5)
        {
            await FileUploadBusiness.SingleChunkFileByArrayBuffer(key, md5);
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
        [HttpGet("chunkfile-finished/{file_md5}/{specs}/{total}")]
        [SwaggerResponse((int)HttpStatusCode.OK, "文件信息", typeof(PersonalFileInfo))]
        public async Task<object> UploadChunkFileFinished(string file_md5, int specs, int total, string type, string extension, string filename)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(FileUploadBusiness.UploadChunkFileFinished(file_md5, specs, total, type, extension, filename))));
        }

        /// <summary>
        /// 通过Base64字符串上传单个图片
        /// </summary>
        /// <param name="base64">Base64字符串</param>
        /// <param name="filename">文件名</param>
        /// <returns></returns>
        [HttpPost("single-image-base64")]
        [SwaggerResponse((int)HttpStatusCode.OK, "文件信息", typeof(PersonalFileInfo))]
        public async Task<object> SingleImageFromBase64([FromBody] string base64, string filename)
        {
            return OpenApiJsonContent(ResponseDataFactory.Success(await FileUploadBusiness.SingleImageFromBase64(base64, filename)));
        }

        /// <summary>
        /// 通过外链上传单个文件
        /// </summary>
        /// <param name="configId">上传配置Id</param>
        /// <param name="url">外链地址</param>
        /// <param name="filename">文件名</param>
        /// <param name="download">是否下载资源</param>
        /// <returns></returns>
        [HttpPost("single-file-url/{configId}")]
        [SwaggerResponse((int)HttpStatusCode.OK, "文件信息", typeof(PersonalFileInfo))]
        public async Task<object> SingleFileFromUrl(string configId, string url, string filename, bool download = false)
        {
            return OpenApiJsonContent(ResponseDataFactory.Success(await FileUploadBusiness.SingleFileFromUrl(configId, url, filename, download)));
        }

        /// <summary>
        /// 上传单个文件
        /// </summary>
        /// <param name="configId">上传配置Id</param>
        /// <param name="file">分片文件</param>
        /// <param name="filename">文件名</param>
        /// <returns></returns>
        [Consumes("multipart/form-data")]
        [HttpPost("single-file/{configId}")]
        [SwaggerResponse((int)HttpStatusCode.OK, "文件信息", typeof(PersonalFileInfo))]
        public async Task<object> SingleFile(string configId, IFormFile file, string filename)
        {
            return OpenApiJsonContent(ResponseDataFactory.Success(await FileUploadBusiness.SingleFile(configId, file, filename)));
        }

        /// <summary>
        /// 上传单个文件
        /// </summary>
        /// <param name="configId">上传配置Id</param>
        /// <param name="type">上传标识</param>
        /// <param name="extension">文件拓展名</param>
        /// <param name="filename">文件名</param>
        /// <returns></returns>
        [Consumes("application/octet-stream")]
        [HttpPost("single-file-arraybuffer/{configId}")]
        [SwaggerResponse((int)HttpStatusCode.OK, "文件信息", typeof(PersonalFileInfo))]
        public async Task<object> SingleFileByArrayBuffer(string configId, string type, string extension, string filename)
        {
            return OpenApiJsonContent(ResponseDataFactory.Success(await FileUploadBusiness.SingleFileByArrayBuffer(configId, type, extension, filename)));
        }
    }
}

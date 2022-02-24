using Business.Utils.Authorization;
using Business.Utils.CAGC;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.Utils.CAGC.CAGCDTO;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Api.Controllers.Utils
{
    /// <summary>
    /// 自动生成代码接口
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "框架拓展")]
    [Route("/cagc")]
    [SampleAuthorize(nameof(ApiPermissionRequirement))]
    [SwaggerTag("自动生成代码接口")]
    public class CAGCController : BaseApiController
    {
        #region DI

        public CAGCController(
            ICAGCBusiness cagcBusiness)
        {
            CAGCBusiness = cagcBusiness;
        }

        readonly ICAGCBusiness CAGCBusiness;

        #endregion

        /// <summary>
        /// 获取自动生成代码应用程序版本信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("version-info")]
        [SwaggerResponse((int)HttpStatusCode.OK, "版本信息", typeof(string))]
        public async Task<object> GetVersionInfo()
        {
            return Success<string>(await CAGCBusiness.GetVersionInfo());
        }

        /// <summary>
        /// 获取所有生成类型
        /// </summary>
        /// <returns></returns>
        [HttpGet("gen-types")]
        [SwaggerResponse((int)HttpStatusCode.OK, "{名称: 值}", typeof(Dictionary<string, string>))]
        public async Task<object> GetGenTypes()
        {
            return await Task.FromResult(Success(CAGCBusiness.GetGenTypes()));
        }

        /// <summary>
        /// 使用CSV文件生成代码
        /// </summary>
        /// <param name="file">CSV文件</param>
        /// <param name="genType">生成类型</param>
        /// <returns></returns>
        [HttpPost("generate-by-csv")]
        [SwaggerResponse((int)HttpStatusCode.OK, "用于下载文件的key", typeof(string))]
        [Consumes("multipart/form-data")]
        public async Task<object> GenerateByCSV(IFormFile file, string genType = "EnrichmentProject")
        {
            return Success<string>(await CAGCBusiness.GenerateByCSV(file, genType));
        }

        /// <summary>
        /// 下载已生成的代码文件
        /// </summary>
        /// <param name="key">用于下载文件的key</param>
        /// <returns></returns>
        [HttpGet("download/{key}")]
        public async Task Download(string key)
        {
            await CAGCBusiness.Download(key);
        }

        /// <summary>
        /// 获取临时文件信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("temp-info")]
        [SwaggerResponse((int)HttpStatusCode.OK, "返回信息", typeof(TempInfo))]
        public async Task<object> GetTempInfo()
        {
            return await Task.FromResult(Success(CAGCBusiness.GetTempInfo()));
        }

        /// <summary>
        /// 清理临时文件
        /// </summary>
        /// <returns></returns>
        [HttpGet("clear-temp")]
        [SwaggerResponse((int)HttpStatusCode.OK, "返回信息", typeof(ClearnTempResult))]
        public async Task<object> ClearTemp()
        {
            return await Task.FromResult(Success(CAGCBusiness.ClearTemp()));
        }
    }
}
using Business.Utils.Authorization;
using Business.Utils.CAGC;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Api.Controllers.Utils
{
    /// <summary>
    /// 自动生成代码接口
    /// </summary>
    [Route("/cagc")]
    [SampleAuthorize(nameof(ApiAuthorizeRequirement))]
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
    }
}
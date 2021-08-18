using AutoMapper;
using Business.Utils.Authorization;
using IocServiceDemo;
using Microservice.Library.DataMapping.Gen;
using Microservice.Library.FreeSql.Gen;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers.Utils
{
    /// <summary>
    /// 测试
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [ApiExplorerSettings(GroupName = "测试")]
    [Route("/test")]
    [SampleAuthorize(nameof(ApiAuthorizeRequirement))]
    [SwaggerTag("测试接口")]
    public class TestV2Controller : BaseApiController
    {
        #region DI

        public TestV2Controller(
            IDemoService demoService,
            IDemoServiceProvider demoServiceProvider,
            IFreeSqlProvider freeSqlProvider,
            IAutoMapperProvider autoMapperProvider)
        {
            if (demoService == null)
                DemoService = demoServiceProvider.GetService();
            else
                DemoService = demoService;

            Orm = freeSqlProvider.GetFreeSql();

            Mapper = autoMapperProvider.GetMapper();
        }

        readonly IDemoService DemoService;

        readonly IMapper Mapper;

        readonly IFreeSql Orm;

        #endregion

        /// <summary>
        /// 获取接口版本
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("api-version")]
        public string GetApiVersion()
        {
            return "2.0";
        }
    }
}
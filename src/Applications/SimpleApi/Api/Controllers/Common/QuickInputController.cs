using Api.Controllers.Utils;
using Business.Interface.Common;
using Business.Utils.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Common.QuickInputDTO;
using Model.Utils.Pagination;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Api.Controllers
{
    /// <summary>
    /// 快捷输入接口
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "框架拓展")]
    [SwaggerTag("快捷输入接口")]
    [Route("/quickinput")]
    [SampleAuthorize(nameof(ApiAuthorizeRequirement))]
    public class QuickInputController : BaseApiController
    {
        #region DI

        public QuickInputController(IQuickInputBusiness quickInputBusiness)
        {
            QuickInputBusiness = quickInputBusiness;
        }

        readonly IQuickInputBusiness QuickInputBusiness;

        #endregion

        #region 基础接口

        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="pagination">分页设置</param>
        /// <returns></returns>
        [HttpPost("list")]
        [SwaggerResponse((int)HttpStatusCode.OK, "列表数据", typeof(List))]
        public async Task<object> GetList([FromBody] PaginationDTO pagination)
        {
            return await Task.FromResult(OpenApiJsonContent(QuickInputBusiness.GetList(pagination), pagination));
        }

        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="category">分类</param>
        /// <param name="keyword">关键词</param>
        /// <param name="paging">是否分页</param>
        /// <param name="rows">每页数据量</param>
        /// <param name="page">页码</param>
        /// <returns></returns>
        [HttpGet("list")]
        [SwaggerResponse((int)HttpStatusCode.OK, "列表数据", typeof(List))]
        public async Task<object> GetCurrentAccountMatchList(string category, string keyword, bool paging = false, int rows = 50, int page = 1)
        {
            return await Task.FromResult(OpenApiJsonContent(QuickInputBusiness.GetCurrentAccountMatchList(category, keyword, paging, rows, page)));
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        [HttpPost("create")]
        public async Task<object> Create([FromBody] Create data)
        {
            QuickInputBusiness.Create(data);
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="datas">数据</param>
        /// <returns></returns>
        [HttpPost("create")]
        public async Task<object> Create([FromBody] IEnumerable<Create> datas)
        {
            QuickInputBusiness.Create(datas.ToList());
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids">Id集合</param>
        /// <returns></returns>
        [HttpPost("delete")]
        [NoJsonParamter]
        public async Task<object> Delete(IEnumerable<string> ids)
        {
            QuickInputBusiness.Delete(ids?.ToList());
            return await Task.FromResult(Success());
        }

        #endregion

        #region 拓展接口



        #endregion        
    }
}

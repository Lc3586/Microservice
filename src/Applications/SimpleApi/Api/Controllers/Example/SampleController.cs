using Api.Controllers.Utils;
using Business.Interface.Example;
using Business.Utils.Authorization;
using Microservice.Library.Extension;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.Example.DBDTO;
using Model.Utils.OfficeDocuments;
using Model.Utils.OfficeDocuments.ExcelDTO;
using Model.Utils.Pagination;
using Model.Utils.Result;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Api.Controllers
{
    /// <summary>
    /// 示例接口
    /// </summary>
    [Route("/sample")]//路由模板
    [SampleAuthorize(nameof(ApiAuthorizeRequirement))]
    [SwaggerTag("示例接口，包含列表、增、删、改、查等接口")]//swagger标签
    public class SampleController : BaseApiController//继承接口基本控制器
    {
        #region DI

        /// <summary>
        /// 在构造函数中注入DI系统中注册的依赖
        /// </summary>
        /// <param name="sampleBusiness"></param>
        public SampleController(ISampleBusiness sampleBusiness)
        {
            SampleBusiness = sampleBusiness;
        }

        /// <summary>
        /// 业务类
        /// </summary>
        readonly ISampleBusiness SampleBusiness;

        #endregion

        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <remarks>
        /// ## 示例 1     一般查询
        /// 
        /// #查询第一页，每页10条数据，按修改时间倒序排序。
        /// 
        ///     POST /config/list
        ///     {
        ///         "PageIndex": 1,
        ///         "PageRows": 10,
        ///         "SortField": "ModifyTime",
        ///         "SortType": "desc"
        ///     }
        /// 
        /// ## 示例 2     高级排序
        /// 
        /// #查询第一页，每页10条数据，按修改时间倒序排序之后再按创建时间正序排列。
        ///
        ///     POST /config/list
        ///     {
        ///         "PageIndex": 1,
        ///         "PageRows": 10,
        ///         "AdvancedSort": [
        ///             {
        ///                 "field": "ModifyTime",
        ///                 "type": "desc"
        ///             },
        ///             {
        ///                 "field": "CreateTime",
        ///                 "type": "asc"
        ///             }
        ///         ]
        ///     }
        /// 
        /// ## 示例 3     高级搜索 1
        /// 
        /// #查询第一页，每页10条数据，筛选应用名称中包含“应用”，以及创建者为“管理员A”的数据。
        /// 
        ///     POST /config/list
        ///     {
        ///         "PageIndex": 1,
        ///         "PageRows": 10,
        ///         "Filter": [
        ///             {
        ///                 "field": "AppName",
        ///                 "value": "应用",
        ///                 "compare": "in"
        ///             },
        ///             {
        ///                 "field": "CreatorName",
        ///                 "value": "管理员A",
        ///                 "compare": "eq"
        ///             }
        ///         ]
        ///     }
        /// 
        /// ## 示例 4     高级搜索 2
        /// 
        /// #查询第一页，每页10条数据，筛选应用名称中包含“应用”，并且创建者为“管理员A”，又或者创建时间大于“2020-03-10”的数据。
        /// 
        ///     POST /config/list
        ///     {
        ///         "PageIndex": 1,
        ///         "PageRows": 10,
        ///         "Filter": [
        ///             {
        ///                 "group": "start",
        ///                 "field": "AppName",
        ///                 "value": "应用",
        ///                 "compare": "in"
        ///             },
        ///             {
        ///                 "group": "end",
        ///                 "relation": "and"
        ///                 "field": "CreatorName",
        ///                 "value": "管理员A",
        ///                 "compare": "eq"
        ///             },
        ///             {
        ///                 "relation": "or"
        ///                 "field": "CreateTime",
        ///                 "value": "2020-03-10",
        ///                 "compare": "gt"
        ///             }
        ///         ]
        ///     }
        /// </remarks>
        /// <param name="pagination">分页设置</param>
        /// <returns></returns>
        [HttpPost("list")]//请求方法以及模板名称
        [SwaggerResponse((int)HttpStatusCode.OK, "列表数据", typeof(List))]//指定输出架构
        public async Task<object> GetList([FromBody]/*指定参数来自请求正文*/PaginationDTO pagination)
        {
            return await Task.FromResult(OpenApiJsonContent(SampleBusiness.GetList(pagination), pagination));
        }

        /// <summary>
        /// 详情数据
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        [HttpPost("detail-data/{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, "详情数据", typeof(Detail))]
        public async Task<object> GetDetail(string id)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(SampleBusiness.GetDetail(id))));
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="data">表单数据</param>
        /// <returns></returns>
        [HttpPost("create")]
        public async Task<object> Create([FromBody]/*指定参数来自请求正文*/Create data)
        {
            SampleBusiness.Create(data);
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 获取编辑数据
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        [HttpPost("edit-data/{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, "编辑数据", typeof(Edit))]
        public async Task<object> GetEdit(string id)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(SampleBusiness.GetEdit(id))));
        }

        /// <summary>
        /// 编辑数据
        /// </summary>
        /// <param name="data">表单数据</param>
        /// <returns></returns>
        [HttpPost("edit")]
        public async Task<object> Edit([FromBody] Edit data)
        {
            SampleBusiness.Edit(data);
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="ids">Id集合</param>
        /// <returns></returns>
        [HttpPost("delete")]
        public async Task<object> Delete(IEnumerable<string> ids)
        {
            SampleBusiness.Delete(ids?.ToList());
            return await Task.FromResult(Success());
        }

        #region 拓展

        /// <summary>
        /// 启用/禁用
        /// </summary>
        /// <param name="id">数据</param>
        /// <param name="enable">设置状态</param>
        /// <returns></returns>
        [HttpPost("enable/{id}/{enable}")]
        public async Task<object> Enable(string id, bool enable)
        {
            SampleBusiness.Enable(id, enable);
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 下载导入模板
        /// </summary>
        /// <param name="version">
        /// Excel文件版本,
        /// xls(2003),
        /// (默认)xlsx(2007)
        /// </param>
        /// <returns></returns>
        [HttpGet("downloadtemplate")]
        public async Task DownloadTemplate(string version = ExcelVersion.xlsx)
        {
            await SampleBusiness.DownloadTemplate(version);
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="file">Execl文件</param>
        /// <returns></returns>
        [HttpPost("import")]
        [SwaggerResponse((int)HttpStatusCode.OK, "导入结果", typeof(ImportResult))]
        [Consumes("multipart/form-data")]
        public async Task<object> Import(IFormFile file)
        {
            return await Task.FromResult(ResponseDataFactory.Success(SampleBusiness.Import(file)));
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="version">
        /// Excel文件版本,
        /// <see cref="ExcelVersion.xls"/>2003,(默认)
        /// <seealso cref="ExcelVersion.xlsx"/>2007
        /// </param>
        /// <param name="paginationJson">分页参数Json字符串</param>
        /// <returns></returns>
        [HttpGet("export")]
        public void Export(string version = ExcelVersion.xlsx, string paginationJson = null)
        {
            SampleBusiness.Export(version, paginationJson);
        }

        #endregion
    }
}

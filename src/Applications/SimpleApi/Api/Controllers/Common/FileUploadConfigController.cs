
/*\__________________________________________________________________________________________________
|*												提示											 __≣|
|* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~| 
|*      此代码由T4模板自动生成																	|
|*		版本:v0.0.0.1                                                                           |__
|*		日期:2021-08-23 15:57:37                                                                   ≣|
|*																				by  LCTR		   ≣|
|* ________________________________________________________________________________________________≣|
\*/



using Api.Controllers.Utils;
using Business.Interface.Common;
using Business.Utils.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.Common.FileUploadConfigDTO;
using Model.Utils.OfficeDocuments;
using Model.Utils.OfficeDocuments.ExcelDTO;
using Model.Utils.Pagination;
using Model.Utils.Result;
using Model.Utils.Sort.SortParamsDTO;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Api.Controllers.Common
{
    /// <summary>
    /// 文件上传配置接口
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "文件管理")]
    [Route("/file-upload-config")]
    [SampleAuthorize(nameof(ApiAuthorizeRequirement))]
    [SwaggerTag("文件上传配置接口")]
    public class FileUploadConfigController : BaseApiController
    {
        #region DI

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileuploadconfigBusiness"></param>
        public FileUploadConfigController(IFileUploadConfigBusiness fileuploadconfigBusiness)
        {
            FileUploadConfigBusiness = fileuploadconfigBusiness;
        }

        /// <summary>
        /// 文件上传配置接口类
        /// </summary>
        readonly IFileUploadConfigBusiness FileUploadConfigBusiness;

        #endregion

        #region 基础功能

        /// <summary>
        /// 获取树状列表数据
        /// </summary>
        /// <param name="pagination">分页设置</param>
        /// <returns></returns>
        [HttpPost("tree-list")]
        [SwaggerResponse((int)HttpStatusCode.OK, "树状列表数据", typeof(List))]
        public async Task<object> GetTreeList([FromBody] TreePaginationDTO pagination)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(FileUploadConfigBusiness.GetTreeList(pagination))));
        }

        /// <summary>
        /// 详情数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpPost("detail-data/{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, "详情数据", typeof(Detail))]
        public async Task<object> GetDetail(string id)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(FileUploadConfigBusiness.GetDetail(id))));
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="data">表单数据</param>
        /// <returns></returns>
        [HttpPost("create")]
        public async Task<object> Create([FromBody] Create data)
        {
            FileUploadConfigBusiness.Create(data);
            return await Task.FromResult(Success("添加成功."));
        }

        /// <summary>
        /// 获取编辑数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpPost("edit-data/{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, "编辑数据", typeof(Edit))]
        public async Task<object> GetEdit(string id)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(FileUploadConfigBusiness.GetEdit(id))));
        }

        /// <summary>
        /// 编辑数据
        /// </summary>
        /// <param name="data">表单数据</param>
        /// <returns></returns>
        [HttpPost("edit")]
        public async Task<object> Edit([FromBody] Edit data)
        {
            FileUploadConfigBusiness.Edit(data);
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keys">[主键]</param>
        /// <returns></returns>
        [HttpPost("delete")]
        public async Task<object> Delete(IEnumerable<string> keys)
        {
            FileUploadConfigBusiness.Delete(keys?.ToList());
            return await Task.FromResult(Success());
        }

        #endregion

        #region 拓展功能

        /// <summary>
        /// 普通排序
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        [HttpPost("sort")]
        public async Task<object> Sort([FromBody] Sort data)
        {
            FileUploadConfigBusiness.Sort(data);
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 拖动排序
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        [HttpPost("dragsort")]
        public async Task<object> DragSort([FromBody] TreeDragSort data)
        {
            FileUploadConfigBusiness.DragSort(data);
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 下载导入模板
        /// </summary>
        /// <param name="version">
        /// <para>指定Excel文件版本</para>
        /// <para><see cref="ExcelVersion.xls"/>: 2003版本</para>
        /// <para>(默认)<see cref="ExcelVersion.xlsx"/>: 2007及以上版本</para>
        /// </param>
        /// <param name="autogenerateTemplate">
        /// <para>指明要使用的模板类型</para>
        /// <para>(默认)true: 自动生成模板</para>
        /// <para>false: 使用预制模板</para>
        /// </param>
        /// <returns></returns>
        [HttpGet("downloadtemplate")]
        public async Task DownloadTemplate(string version = ExcelVersion.xlsx, bool autogenerateTemplate = true)
        {
            await FileUploadConfigBusiness.DownloadTemplate(version, autogenerateTemplate);
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="file">Execl文件</param>
        /// <param name="autogenerateTemplate">
        /// <para>指明所使用的模板类型</para>
        /// <para>(默认)true: 自动生成的模板</para>
        /// <para>false: 预制模板</para>
        /// </param>
        /// <returns></returns>
        [HttpPost("import")]
        [SwaggerResponse((int)HttpStatusCode.OK, "导入结果", typeof(ImportResult))]
        [Consumes("multipart/form-data")]
        public async Task<object> Import(IFormFile file, bool autogenerateTemplate = true)
        {
            return await Task.FromResult(ResponseDataFactory.Success(FileUploadConfigBusiness.Import(file, autogenerateTemplate)));
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="version">
        /// <para>指定Excel文件版本</para>
        /// <para><see cref="ExcelVersion.xls"/>: 2003版本</para>
        /// <para>(默认)<see cref="ExcelVersion.xlsx"/>: 2007及以上版本</para>
        /// </param>
        /// <param name="paginationJson">分页参数Json字符串</param>
        /// <returns></returns>
        [HttpGet("export")]
        public void Export(string version = ExcelVersion.xlsx, string paginationJson = null)
        {
            FileUploadConfigBusiness.Export(version, paginationJson);
        }

        /// <summary>
        /// 配置数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpPost("data/{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, "配置数据", typeof(Config))]
        public async Task<object> GetConfig(string id)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(FileUploadConfigBusiness.GetConfig(id))));
        }

        /// <summary>
        /// 获取配置数据集合
        /// </summary>
        /// <param name="ids">id集合</param>
        /// <returns></returns>
        [HttpPost("list")]
        [Consumes("application/json", "application/x-www-form-urlencoded")]
        [Produces("application/json")]
        [SwaggerResponse((int)HttpStatusCode.OK, "配置数据", typeof(Config))]
        public async Task<object> GetConfigs(List<string> ids)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(FileUploadConfigBusiness.GetConfigs(ids))));
        }

        #endregion
    }
}


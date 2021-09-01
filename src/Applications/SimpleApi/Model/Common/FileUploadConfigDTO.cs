
/*\__________________________________________________________________________________________________
|*												提示											 __≣|
|* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~| 
|*      此代码由T4模板自动生成																	|
|*		版本:v0.0.0.1                                                                           |__
|*		日期:2021-08-19 17:09:55                                                                   ≣|
|*																				by  LCTR		   ≣|
|* ________________________________________________________________________________________________≣|
\*/
using Entity.Common;
using Microservice.Library.DataMapping.Annotations;
using Microservice.Library.OpenApi.Annotations;
using System.Collections.Generic;
using System.ComponentModel;

/* 
 * 文件上传配置业务模型
 */
namespace Model.Common.FileUploadConfigDTO
{
    /// <summary>
    /// 新增
    /// </summary>
    [MapFrom(typeof(Common_FileUploadConfig))]
    [OpenApiMainTag("Create")]
    public class Create : Common_FileUploadConfig
    {

    }

    /// <summary>
    /// 详情
    /// </summary>
    [MapFrom(typeof(Common_FileUploadConfig))]
    [OpenApiMainTag("Detail")]
    public class Detail : Common_FileUploadConfig
    {

    }

    /// <summary>
    /// 编辑
    /// </summary>
    [MapFrom(typeof(Common_FileUploadConfig))]
    [OpenApiMainTag("Edit")]
    public class Edit : Common_FileUploadConfig
    {

    }

    /// <summary>
    /// 导入
    /// </summary>
    [MapFrom(typeof(Common_FileUploadConfig))]
    [OpenApiMainTag("Import")]
    public class Import : Common_FileUploadConfig
    {

    }

    /// <summary>
    /// 导出
    /// </summary>
    [MapFrom(typeof(Common_FileUploadConfig))]
    [OpenApiMainTag("Export")]
    public class Export : Common_FileUploadConfig
    {

    }

    /// <summary>
    /// 列表
    /// </summary>
    [MapFrom(typeof(Common_FileUploadConfig))]
    [OpenApiMainTag("List")]
    public class List : Common_FileUploadConfig
    {
        /// <summary>
        /// 子级
        /// </summary>
        [OpenApiSchema(OpenApiSchemaType.model, OpenApiSchemaFormat.model_once)]
        [Description("子级")]
        public List<List> Childs_ { get; set; }
    }

    /// <summary>
    /// 配置
    /// </summary>
    [MapFrom(typeof(Common_FileUploadConfig))]
    [OpenApiMainTag("Config")]
    public class Config : Common_FileUploadConfig
    {

    }
}


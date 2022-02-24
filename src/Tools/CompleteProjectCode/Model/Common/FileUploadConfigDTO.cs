
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
        /// <summary>
        /// 引用编码
        /// </summary>
        [Description("引用编码")]
        public string ReferenceCode { get; set; }

        /// <summary>
        /// 允许的MIME类型
        /// <para>[,]逗号分隔</para>
        /// <para>此值为空时未禁止即允许</para>
        /// </summary>
        public List<string> AllowedTypeList { get; set; }

        /// <summary>
        /// 禁止的MIME类型
        /// <para>[,]逗号分隔</para>
        /// <para>此值为空时皆可允许</para>
        /// </summary>
        public List<string> ProhibitedTypeList { get; set; }
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
        /// <summary>
        /// 父级编码
        /// </summary>
        [Description("父级编码")]
        public string ParentCode { get; set; }

        /// <summary>
        /// 引用编码
        /// </summary>
        [Description("引用编码")]
        public string ReferenceCode { get; set; }

        /// <summary>
        /// 新增数据
        /// </summary>
        [OpenApiIgnore]
        public bool New { get; set; }
    }

    /// <summary>
    /// 导出
    /// </summary>
    [MapFrom(typeof(Common_FileUploadConfig))]
    [OpenApiMainTag("Export")]
    public class Export : Common_FileUploadConfig
    {
        /// <summary>
        /// 父级编码
        /// </summary>
        [Description("父级编码")]
        public string ParentCode { get; set; }

        /// <summary>
        /// 引用编码
        /// </summary>
        [Description("引用编码")]
        public string ReferenceCode { get; set; }
    }

    /// <summary>
    /// 列表
    /// </summary>
    [MapFrom(typeof(Common_FileUploadConfig))]
    [OpenApiMainTag("List")]
    public class List : Common_FileUploadConfig
    {
        /// <summary>
        /// 是否拥有子级
        /// </summary>
        public bool HasChildren { get; set; }

        /// <summary>
        /// 子级数量
        /// </summary>
        public int ChildrenCount { get; set; }

        /// <summary>
        /// 子级
        /// </summary>
        [OpenApiSchema(OpenApiSchemaType.model, OpenApiSchemaFormat.model_once)]
        [Description("子级")]
        public List<List> Children { get; set; }
    }

    /// <summary>
    /// 配置
    /// </summary>
    [MapFrom(typeof(Common_FileUploadConfig))]
    [OpenApiMainTag("Config")]
    public class Config : Common_FileUploadConfig
    {
        /// <summary>
        /// 允许的MIME类型
        /// <para>[,]逗号分隔</para>
        /// <para>此值为空时未禁止即允许</para>
        /// </summary>
        public List<string> AllowedTypeList { get; set; }

        /// <summary>
        /// 禁止的MIME类型
        /// <para>[,]逗号分隔</para>
        /// <para>此值为空时皆可允许</para>
        /// </summary>
        public List<string> ProhibitedTypeList { get; set; }
    }

    /// <summary>
    /// 类型信息
    /// </summary>
    public class Types
    {
        /// <summary>
        /// 允许的MIME类型
        /// <para>[,]逗号分隔</para>
        /// <para>此值为空时未禁止即允许</para>
        /// </summary>
        [Description("允许的MIME类型")]
        public string AllowedTypes { get; set; }

        /// <summary>
        /// 禁止的MIME类型
        /// <para>[,]逗号分隔</para>
        /// <para>此值为空时皆可允许</para>
        /// </summary>
        [Description("禁止的MIME类型")]
        public string ProhibitedTypes { get; set; }
    }

    /// <summary>
    /// 授权信息
    /// </summary>
    [MapFrom(typeof(Common_FileUploadConfig))]
    [OpenApiMainTag("Authorities")]
    public class Authorities : Common_FileUploadConfig
    {

    }

    /// <summary>
    /// 授权信息树状列表
    /// </summary>
    [MapFrom(typeof(Common_FileUploadConfig))]
    [OpenApiMainTag("Authorities")]
    public class AuthoritiesTree : Common_FileUploadConfig
    {
        /// <summary>
        /// 已授权
        /// </summary>
        public bool Authorized { get; set; }

        /// <summary>
        /// 是否拥有子级
        /// </summary>
        public bool HasChildren { get; set; }

        /// <summary>
        /// 子级数量
        /// </summary>
        public int ChildrenCount { get; set; }

        /// <summary>
        /// 子级授权信息
        /// </summary>
        [OpenApiSchema(OpenApiSchemaType.model, OpenApiSchemaFormat.model_once)]
        [Description("子级授权信息")]
        public List<AuthoritiesTree> Children { get; set; }
    }
}


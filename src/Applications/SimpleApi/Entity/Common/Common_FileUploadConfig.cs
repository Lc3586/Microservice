
/*\__________________________________________________________________________________________________
|*												提示											 __≣|
|* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~| 
|*      此代码由T4模板自动生成																	|
|*		版本:v0.0.0.1                                                                           |__
|*		日期:2021-08-19 17:09:55                                                                   ≣|
|*																				by  LCTR		   ≣|
|* ________________________________________________________________________________________________≣|
\*/



using Entity.System;
using FreeSql.DataAnnotations;
using Microservice.Library.OpenApi.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Entity.Common
{
    /// <summary>
    /// 文件上传配置
    /// </summary>
    [Index("Common_FileUploadConfig_idx_01", nameof(RootId) + " ASC," + nameof(ParentId) + " ASC," + nameof(Name) + " ASC," + nameof(DisplayName) + " ASC," + nameof(Public) + " DESC" + nameof(Level) + " ASC," + nameof(Sort) + " ASC," + nameof(Enable) + " DESC" + nameof(CreatorId) + " ASC," + nameof(CreateTime) + " DESC," + nameof(ModifyTime) + " DESC")]
    [OraclePrimaryKeyName("pk_Common_FileUploadConfig_Id")]
    [Table]
    public class Common_FileUploadConfig
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Column(IsPrimary = true, StringLength = 36)]
        [OpenApiSubTag("List", "Edit", "Detail", "Config", "Authorities")]
        public string Id { get; set; }

        /// <summary>
        /// 根Id
        /// </summary>
        [Column(StringLength = 36)]
        [OpenApiSubTag("List", "Detail", "_Import")]
        public string RootId { get; set; }

        /// <summary>
        /// 父级Id
        /// </summary>
        [Column(StringLength = 36)]
        [OpenApiSubTag("List", "Create", "Detail", "_Import", "_Export")]
        public string ParentId { get; set; }

        /// <summary>
        /// 层级
        /// </summary>
        [Description("层级")]
        [OpenApiSubTag("List", "Detail", "_Import")]
        public int Level { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        [Column(StringLength = 36)]
        [OpenApiSubTag("List", "Create", "Edit", "Detail", "Import", "Export", "Authorities")]
        [Description("编码")]
        [Required(ErrorMessage = "编码不可为空")]
        public string Code { get; set; }

        /// <summary>
        /// 引用的上传配置Id
        /// <para>引用文件MIME类型，会合并当前数据以及引用数据</para>
        /// </summary>
        [Column(StringLength = 36)]
        [OpenApiSubTag("List", "Create", "Edit", "Detail", "_Import", "_Export")]
        public string ReferenceId { get; set; }

        /// <summary>
        /// 级联引用
        /// <para>使用引用的上传配置以及它的所有子集配置</para>
        /// </summary>
        [Column(IsNullable = true)]
        [OpenApiSubTag("List", "Create", "Edit", "Detail", "Import", "Export")]
        [Description("级联引用")]
        public bool ReferenceTree { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Column(StringLength = 50)]
        [Description("名称")]
        [OpenApiSubTag("List", "Create", "Edit", "Detail", "Import", "Export", "Authorities")]
        [Required(ErrorMessage = "名称不可为空")]
        public string Name { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        [Column(StringLength = 50)]
        [Description("显示名称")]
        [OpenApiSubTag("List", "Create", "Edit", "Detail", "Import", "Export", "Authorities")]
        [Required(ErrorMessage = "显示名称不可为空")]
        public string DisplayName { get; set; }

        /// <summary>
        /// 公共配置
        /// <para>无需授权</para>
        /// </summary>
        [Description("公共配置")]
        [OpenApiSubTag("List", "Create", "Edit", "Detail", "Import", "Export", "Authorities")]
        public bool Public { get; set; }

        /// <summary>
        /// 文件数量下限
        /// </summary>
        [Description("文件数量下限")]
        [Range(0, int.MaxValue, ErrorMessage = "文件数量下限必须大于或等于0")]
        [OpenApiSubTag("List", "Create", "Edit", "Detail", "Import", "Export", "Config")]
        public int LowerLimit { get; set; }

        /// <summary>
        /// 文件数量上限
        /// </summary>
        [Description("文件数量上限")]
        [Range(1, int.MaxValue, ErrorMessage = "文件数量上限限必须大于或等于1")]
        [OpenApiSubTag("List", "Create", "Edit", "Detail", "Import", "Export", "Config")]
        public int UpperLimit { get; set; }

        /// <summary>
        /// 允许的MIME类型
        /// <para>[,]逗号分隔</para>
        /// <para>此值为空时未禁止即允许</para>
        /// </summary>
        [Column(StringLength = -2)]
        [Description("允许的MIME类型")]
        [OpenApiSubTag("Create", "Edit", "Detail", "Import", "Export")]
        public string AllowedTypes { get; set; }

        /// <summary>
        /// 禁止的MIME类型
        /// <para>[,]逗号分隔</para>
        /// <para>此值为空时皆可允许</para>
        /// </summary>
        [Column(StringLength = -2)]
        [Description("禁止的MIME类型")]
        [OpenApiSubTag("Create", "Edit", "Detail", "Import", "Export")]
        public string ProhibitedTypes { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        [Column(StringLength = -2)]
        [Description("说明")]
        [OpenApiSubTag("Create", "Edit", "Detail", "Import", "Export", "Config")]
        public string Explain { get; set; }

        /// <summary>
        /// 排序值
        /// </summary>
        [Description("排序值")]
        [OpenApiSubTag("List", "Sort", "_Import")]
        public int Sort { get; set; }

        /// <summary>
        /// 启用
        /// </summary>
        [Description("启用")]
        [OpenApiSubTag("List", "Create", "Edit", "Detail", "Import", "Export")]
        public bool Enable { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Column(StringLength = -2)]
        [Description("备注")]
        [OpenApiSubTag("Create", "Edit", "Detail", "Import", "Export")]
        public string Remark { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        [Column(StringLength = 36)]
        public string CreatorId { get; set; }

        /// <summary>
        /// 创建者名称
        /// </summary>
        [Column(StringLength = 50)]
        [Description("创建者")]
        [OpenApiSubTag("Detail")]
        public string CreatorName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Description("创建时间")]
        [OpenApiSubTag("List", "Detail")]
        [OpenApiSchema(OpenApiSchemaType.@string, OpenApiSchemaFormat.string_datetime)]
        [JsonConverter(typeof(Microservice.Library.OpenApi.JsonExtension.DateTimeConverter), "yyyy-MM-dd HH:mm:ss")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 最近编辑者
        /// </summary>
        [Column(StringLength = 36)]
        [OpenApiSubTag("_Edit")]
        public string ModifiedById { get; set; }

        /// <summary>
        /// 最近编辑者名称
        /// </summary>
        [Column(StringLength = 50)]
        [Description("最近编辑者")]
        [OpenApiSubTag("Detail", "_Edit")]
        public string ModifiedByName { get; set; }

        /// <summary>
        /// 最近编辑时间
        /// </summary>
        [Column(IsNullable = true)]
        [Description("最近编辑时间")]
        [OpenApiSubTag("List", "Detail", "_Edit")]
        [OpenApiSchema(OpenApiSchemaType.@string, OpenApiSchemaFormat.string_datetime)]
        [JsonConverter(typeof(Microservice.Library.OpenApi.JsonExtension.DateTimeConverter), "yyyy-MM-dd HH:mm:ss")]
        public DateTime? ModifyTime { get; set; }

        #region 关联

        /// <summary>
        /// 创建者
        /// </summary>
        [JsonIgnore]
        [Navigate(nameof(CreatorId))]
        [OpenApiIgnore]
        [XmlIgnore]
        public virtual System_User CreatorUser { get; set; }

        /// <summary>
        /// 最近编辑者
        /// </summary>
        [JsonIgnore]
        [Navigate(nameof(ModifiedById))]
        [OpenApiIgnore]
        [XmlIgnore]
        public virtual System_User ModifiedBuUser { get; set; }

        /// <summary>
        /// 根配置
        /// </summary>
        [JsonIgnore]
        [Navigate(nameof(RootId))]
        [OpenApiIgnore]
        [XmlIgnore]
        public virtual Common_FileUploadConfig RootConfig { get; set; }

        /// <summary>
        /// 父级配置
        /// </summary>
        [JsonIgnore]
        [Navigate(nameof(ParentId))]
        [OpenApiIgnore]
        [XmlIgnore]
        public virtual Common_FileUploadConfig ParentConfig { get; set; }

        /// <summary>
        /// 子级配置
        /// </summary>
        [JsonIgnore]
        [Navigate(nameof(ParentId))]
        [OpenApiIgnore]
        [XmlIgnore]
        public virtual ICollection<Common_FileUploadConfig> ChildConfigs { get; set; }

        /// <summary>
        /// 引用配置
        /// </summary>
        [JsonIgnore]
        [Navigate(nameof(ReferenceId))]
        [OpenApiIgnore]
        [XmlIgnore]
        public virtual Common_FileUploadConfig ReferenceConfig { get; set; }

        /// <summary>
        /// 被直接授权了此配置的用户
        /// </summary>
        [JsonIgnore]
        [Navigate(ManyToMany = typeof(System_UserCFUC))]
        [OpenApiIgnore]
        [XmlIgnore]
        public virtual ICollection<System_User> System_Users { get; set; }

        /// <summary>
        /// 被授权了此配置的角色
        /// </summary>
        [JsonIgnore]
        [Navigate(ManyToMany = typeof(System_RoleCFUC))]
        [OpenApiIgnore]
        [XmlIgnore]
        public virtual ICollection<System_Role> System_Roles { get; set; }

        /// <summary>
        /// 使用此配置上传的个人文件信息
        /// </summary>
        [JsonIgnore]
        [Navigate(nameof(Common_PersonalFileInfo.ConfigId))]
        [OpenApiIgnore]
        [XmlIgnore]
        public virtual ICollection<Common_PersonalFileInfo> PersonalFileInfos { get; set; }

        #endregion
    }
}


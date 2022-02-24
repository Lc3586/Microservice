using Entity.Public;
using Entity.System;
using FreeSql.DataAnnotations;
using Microservice.Library.OpenApi.Annotations;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Entity.Common
{
    /// <summary>
    /// 个人文件信息
    /// </summary>
    [Table]
    [OraclePrimaryKeyName("pk_C_PFI")]
    [Index("C_PFI_idx_01", nameof(FileId) + " ASC")]
    [Index("C_PFI_idx_01", nameof(Name) + " ASC")]
    [Index("C_PFI_idx_01", nameof(State) + " ASC")]
    [Index("C_PFI_idx_07", nameof(CreatorId) + " ASC")]
    [Index("C_PFI_idx_08", nameof(CreateTime) + " DESC")]
    public class Common_PersonalFileInfo
    {
        /// <summary>
        /// Id
        /// </summary>
        [OpenApiSubTag("List", "Detail", "PersonalFileInfo")]
        [Column(IsPrimary = true, StringLength = 36)]
        public string Id { get; set; }

        /// <summary>
        /// 文件Id
        /// </summary>
        [OpenApiSubTag("List", "Detail", "PersonalFileInfo")]
        [Column(StringLength = 36)]
        public string FileId { get; set; }

        /// <summary>
        /// 文件上传配置Id
        /// </summary>
        [OpenApiSubTag("List", "Detail", "PersonalFileInfo")]
        [Column(StringLength = 36)]
        public string ConfigId { get; set; }

        /// <summary>
        /// 文件重命名
        /// </summary>
        [OpenApiSubTag("List", "Detail", "Edit", "PersonalFileInfo")]
        [Column(StringLength = 256)]
        public string Name { get; set; }

        /// <summary>
        /// 文件扩展名
        /// </summary>
        [OpenApiSubTag("List", "Detail", "PersonalFileInfo")]
        [Column(StringLength = 256)]
        public string Extension { get; set; }

        /// <summary>
        /// 类别
        /// </summary>
        [OpenApiSubTag("List", "Detail", "Edit", "PersonalFileInfo")]
        [Column(StringLength = 256)]
        public string Category { get; set; }

        /// <summary>
        /// 星级
        /// </summary>
        [OpenApiSubTag("List", "Detail", "Edit", "PersonalFileInfo")]
        [Column]
        public int Star { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        [OpenApiSubTag("List", "Detail", "Edit", "PersonalFileInfo")]
        [Column(StringLength = 1000)]
        public string Tags { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [OpenApiSubTag("List", "Detail", "PersonalFileInfo")]
        [Column(StringLength = 10)]
        public string State { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        [OpenApiSubTag("_List")]
        [Column(StringLength = 50)]
        public string CreatorId { get; set; }

        /// <summary>
        /// 创建者名称
        /// </summary>
        [OpenApiSubTag("List", "Detail")]
        [Column(StringLength = 30)]
        public string CreatorName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [OpenApiSubTag("List", "Detail", "PersonalFileInfo")]
        [OpenApiSchema(OpenApiSchemaType.@string, OpenApiSchemaFormat.string_datetime)]
        [JsonConverter(typeof(Microservice.Library.OpenApi.JsonExtension.DateTimeConverter), "yyyy-MM-dd HH:mm:ss")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 最近编辑者
        /// </summary>
        [OpenApiSubTag("_Edit")]
        [Column(StringLength = 36)]
        public string ModifiedById { get; set; }

        /// <summary>
        /// 最近编辑者名称
        /// </summary>
        [OpenApiSubTag("List", "Detail", "_Edit")]
        [Description("最近编辑者")]
        [Column(StringLength = 50)]
        public string ModifiedByName { get; set; }

        /// <summary>
        /// 最近编辑时间
        /// </summary>
        [OpenApiSubTag("List", "Detail", "_Edit", "PersonalFileInfo")]
        [OpenApiSchema(OpenApiSchemaType.@string, OpenApiSchemaFormat.string_datetime)]
        [JsonConverter(typeof(Microservice.Library.OpenApi.JsonExtension.DateTimeConverter), "yyyy-MM-dd HH:mm:ss")]
        [Description("最近编辑时间")]
        [Column(IsNullable = true)]
        public DateTime? ModifyTime { get; set; }

        #region 关联

        /// <summary>
        /// 所属用户
        /// </summary>
        [Navigate(nameof(CreatorId))]
        [OpenApiIgnore]
        [JsonIgnore]
        [XmlIgnore]
        public virtual System_User User { get; set; }

        /// <summary>
        /// 所属会员
        /// </summary>
        [Navigate(nameof(CreatorId))]
        [OpenApiIgnore]
        [JsonIgnore]
        [XmlIgnore]
        public virtual Public_Member Member { get; set; }

        /// <summary>
        /// 文件信息
        /// </summary>
        [Navigate(nameof(FileId))]
        [OpenApiIgnore]
        [JsonIgnore]
        [XmlIgnore]
        public virtual Common_File Common_File { get; set; }

        /// <summary>
        /// 上传时使用的配置
        /// </summary>
        [JsonIgnore]
        [Navigate(nameof(ConfigId))]
        [OpenApiIgnore]
        [XmlIgnore]
        public virtual Common_FileUploadConfig FileUploadConfig { get; set; }

        #endregion
    }
}

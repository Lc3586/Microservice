using FreeSql.DataAnnotations;
using Microservice.Library.OpenApi.Annotations;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Net5TC.Entity
{
    /// <summary>
    /// 示例实体类（数据库）
    /// </summary>
    /// <remarks>附带导航属性，一对一</remarks>
    [Table]//FreeSql使用CodeFirst模式时必须添加此特性，如果要禁用实体同步设置DisableSyncStructure = true
    [OraclePrimaryKeyName("pk_" + nameof(Example_DB_D))]
    //[System.ComponentModel.DataAnnotations.Schema.Table(nameof(Example_DB_D))]//特别指定数据库表名
    #region 设置索引
    [Index(nameof(Example_DB_D) + "_idx_" + nameof(Name), nameof(Name) + " ASC")]
    [Index(nameof(Example_DB_D) + "_idx_" + nameof(CreatorId), nameof(CreatorId) + " ASC")]
    [Index(nameof(Example_DB_D) + "_idx_" + nameof(CreatorName), nameof(CreatorName) + " ASC")]
    [Index(nameof(Example_DB_D) + "_idx_" + nameof(CreateTime), nameof(CreateTime) + " DESC")]
    #endregion
    public class Example_DB_D
    {
        /// <summary>
        /// Id
        /// </summary>
        [Column(IsPrimary = true)]//设置主键
        public string Id { get; set; }

        /// <summary>
        /// 所属A
        /// </summary>
        [Column(StringLength = 36)]
        public string AId { get; set; }

        /// <summary>
        /// 所属A
        /// </summary>
        /// <remarks>一对一</remarks>
        [Navigate(nameof(AId))]
        [OpenApiIgnore]
        [JsonIgnore]
        [XmlIgnore]
        public virtual Example_DB_A Example_DB_A { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [OpenApiSubTag("List", "Create", "Edit", "Detail")]
        [OpenApiSchema(OpenApiSchemaType.@string)]
        [Required(ErrorMessage = "名称不可为空")]//非空验证
        [Description("名称")]
        [Column(StringLength = 50)]
        public string Name { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        [Column(StringLength = 36)]
        public string CreatorId { get; set; }

        /// <summary>
        /// 创建者名称
        /// </summary>
        [OpenApiSubTag("List", "Detail")]
        [OpenApiSchema(OpenApiSchemaType.@string)]
        [Description("创建者")]
        [Column(StringLength = 50)]
        public string CreatorName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [OpenApiSubTag("List", "Detail")]
        [OpenApiSchema(OpenApiSchemaType.@string, OpenApiSchemaFormat.string_datetime)]
        [JsonConverter(typeof(Microservice.Library.OpenApi.JsonExtension.DateTimeConverter), "yyyy-MM-dd HH:mm:ss")]
        [Description("创建时间")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 最近修改时间
        /// </summary>
        [OpenApiSubTag("List", "Detail")]
        [OpenApiSchema(OpenApiSchemaType.@string, OpenApiSchemaFormat.string_datetime)]
        [JsonConverter(typeof(Microservice.Library.OpenApi.JsonExtension.DateTimeConverter), "yyyy-MM-dd HH:mm:ss")]
        [Description("最近修改时间")]
        public DateTime ModifyTime { get; set; }
    }
}

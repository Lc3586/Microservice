using FreeSql.DataAnnotations;
using Microservice.Library.OpenApi.Annotations;
using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace Entity.Common
{
    /// <summary>
    /// 快捷输入
    /// </summary>
    [Table]
    [OraclePrimaryKeyName("pk_C_QI")]
    #region 设置索引
    [Index("C_QI_idx_01", nameof(Category) + " ASC," + nameof(Keyword) + " ASC")]
    [Index("C_QI_idx_02", nameof(Public) + " ASC")]
    [Index("C_QI_idx_03", nameof(CreatorId) + " ASC," + nameof(CreateTime) + " DESC")]
    #endregion
    public class Common_QuickInput
    {
        /// <summary>
        /// Id
        /// </summary>
        [OpenApiSubTag("List")]
        [Column(IsPrimary = true, StringLength = 36)]
        public string Id { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        [OpenApiSubTag("List", "Create")]
        [Description("分类")]
        [Column(StringLength = 20)]
        public string Category { get; set; }

        /// <summary>
        /// 关键词
        /// </summary>
        [OpenApiSubTag("List", "Create")]
        [Description("关键词")]
        [Column(StringLength = 200)]
        public string Keyword { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [OpenApiSubTag("List", "Create")]
        [Description("内容")]
        [Column(StringLength = 1024)]
        public string Content { get; set; }

        /// <summary>
        /// 公用
        /// </summary>
        [OpenApiSubTag("List", "Create")]
        [Description("公用")]
        public bool Public { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        [Column(StringLength = 36)]
        public string CreatorId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [OpenApiSubTag("List")]
        [OpenApiSchema(OpenApiSchemaType.@string, OpenApiSchemaFormat.string_datetime)]
        [JsonConverter(typeof(DateTimeConverter), "yyyy-MM-dd HH:mm:ss")]
        public DateTime CreateTime { get; set; }
    }
}

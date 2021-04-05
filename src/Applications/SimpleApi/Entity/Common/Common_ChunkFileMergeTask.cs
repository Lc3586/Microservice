using FreeSql.DataAnnotations;
using Microservice.Library.OpenApi.JsonExtension;
using Microservice.Library.OpenApi.Annotations;
using Newtonsoft.Json;
using System;

namespace Entity.Common
{
    /// <summary>
    /// 分片文件合并任务
    /// </summary>
    [Table]
    [OraclePrimaryKeyName("pk_SPAA_" + nameof(Common_ChunkFileMergeTask) + "_01")]
    [Index(nameof(Common_ChunkFileMergeTask) + "_idx_" + nameof(ServerKey), nameof(ServerKey) + " ASC")]
    [Index(nameof(Common_ChunkFileMergeTask) + "_idx_" + nameof(MD5), nameof(MD5) + " ASC")]
    [Index(nameof(Common_ChunkFileMergeTask) + "_idx_" + nameof(Specs), nameof(Specs) + " ASC")]
    [Index(nameof(Common_ChunkFileMergeTask) + "_idx_" + nameof(Total), nameof(Total) + " ASC")]
    [Index(nameof(Common_ChunkFileMergeTask) + "_idx_" + nameof(State), nameof(State) + " ASC")]
    public class Common_ChunkFileMergeTask
    {
        /// <summary>
        /// Id
        /// </summary>
        [OpenApiSubTag("List")]
        [Column(IsPrimary = true, StringLength = 36)]
        public string Id { get; set; }

        /// <summary>
        /// 服务器标识
        /// </summary>
        [OpenApiSubTag("List", "Detail")]
        [Column(StringLength = 36)]
        public string ServerKey { get; set; }

        /// <summary>
        /// 文件MD5校验值
        /// </summary>
        [OpenApiSubTag("List", "Detail")]
        [Column(StringLength = 36)]
        public string MD5 { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [OpenApiSubTag("List", "Detail")]
        [Column(StringLength = 256)]
        public string Name { get; set; }

        /// <summary>
        /// 完整名称
        /// </summary>
        [OpenApiSubTag("List", "Detail", "FileInfo")]
        [Column(StringLength = 300)]
        public string FullName { get; set; }

        /// <summary>
        /// 分片规格
        /// </summary>
        [OpenApiSubTag("List", "Detail")]
        public int Specs { get; set; }

        /// <summary>
        /// 分片总数
        /// </summary>
        [OpenApiSubTag("List", "Detail")]
        public int Total { get; set; }

        /// <summary>
        /// 当前处理分片的索引
        /// </summary>
        [OpenApiSubTag("List", "Detail")]
        public int CurrentChunkIndex { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [OpenApiSubTag("List", "Detail")]
        [Column(StringLength = 10)]
        public string State { get; set; }

        /// <summary>
        /// 信息
        /// </summary>
        [OpenApiSubTag("List", "Detail")]
        [Column(StringLength = 300)]
        public string Info { get; set; }

        /// <summary>
        /// 内容类型
        /// </summary>
        [OpenApiSubTag("List", "Detail")]
        [Column(StringLength = 50)]
        public string ContentType { get; set; }

        /// <summary>
        /// 文件扩展名
        /// </summary>
        [OpenApiSubTag("List", "Detail")]
        [Column(StringLength = 30)]
        public string Extension { get; set; }

        /// <summary>
        /// 字节数
        /// </summary>
        [OpenApiSubTag("List", "Detail")]
        [Column(IsNullable = true)]
        public long? Bytes { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        [OpenApiSubTag("List", "Detail")]
        [Column(IsNullable = true)]
        public string Size { get; set; }

        /// <summary>
        /// 合并后的文件路径
        /// </summary>
        [OpenApiSubTag("List", "Detail")]
        [Column(StringLength = -1)]
        public string Path { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [OpenApiSubTag("List", "Detail")]
        [OpenApiSchema(OpenApiSchemaType.@string, OpenApiSchemaFormat.string_datetime)]
        [JsonConverter(typeof(DateTimeConverter), "yyyy-MM-dd HH:mm:ss")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 最近更新时间
        /// </summary>
        [OpenApiSubTag("List", "Detail")]
        [OpenApiSchema(OpenApiSchemaType.@string, OpenApiSchemaFormat.string_datetime)]
        [JsonConverter(typeof(DateTimeConverter), "yyyy-MM-dd HH:mm:ss")]
        [Column(IsNullable = true)]
        public DateTime? ModifyTime { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        [OpenApiSubTag("List", "Detail")]
        [OpenApiSchema(OpenApiSchemaType.@string, OpenApiSchemaFormat.string_datetime)]
        [JsonConverter(typeof(DateTimeConverter), "yyyy-MM-dd HH:mm:ss")]
        [Column(IsNullable = true)]
        public DateTime? CompletedTime { get; set; }
    }
}

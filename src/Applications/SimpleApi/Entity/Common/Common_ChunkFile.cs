using FreeSql.DataAnnotations;
using Microservice.Library.OpenApi.JsonExtension;
using Microservice.Library.OpenApi.Annotations;
using Newtonsoft.Json;
using System;

namespace Entity.Common
{
    /// <summary>
    /// 分片文件信息
    /// </summary>
    [Table]
    [OraclePrimaryKeyName("pk_SPAA_" + nameof(Common_ChunkFile) + "_01")]
    [Index(nameof(Common_ChunkFile) + "_idx_" + nameof(ServerKey), nameof(ServerKey) + " ASC")]
    [Index(nameof(Common_ChunkFile) + "_idx_" + nameof(TaskKey), nameof(TaskKey) + " ASC")]
    [Index(nameof(Common_ChunkFile) + "_idx_" + nameof(FileMD5), nameof(FileMD5) + " ASC")]
    [Index(nameof(Common_ChunkFile) + "_idx_" + nameof(MD5), nameof(MD5) + " ASC")]
    [Index(nameof(Common_ChunkFile) + "_idx_" + nameof(Index), nameof(Index) + " ASC")]
    [Index(nameof(Common_ChunkFile) + "_idx_" + nameof(Specs), nameof(Specs) + " ASC")]
    [Index(nameof(Common_ChunkFile) + "_idx_" + nameof(State), nameof(State) + " ASC")]
    public class Common_ChunkFile
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
        /// 任务标识
        /// </summary>
        [OpenApiSubTag("List", "Detail")]
        [Column(StringLength = 36)]
        public string TaskKey { get; set; }

        /// <summary>
        /// 文件MD5校验值
        /// </summary>
        [OpenApiSubTag("List", "Detail")]
        [Column(StringLength = 36)]
        public string FileMD5 { get; set; }

        /// <summary>
        /// 分片MD5校验值
        /// </summary>
        [OpenApiSubTag("List", "Detail")]
        [Column(StringLength = 36)]
        public string MD5 { get; set; }

        /// <summary>
        /// 分片索引
        /// </summary>
        [OpenApiSubTag("List", "Detail")]
        public int Index { get; set; }

        /// <summary>
        /// 分片规格
        /// </summary>
        [OpenApiSubTag("List", "Detail")]
        public int Specs { get; set; }

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
        /// 文件路径
        /// </summary>
        [OpenApiSubTag("_List")]
        [Column(StringLength = -1)]
        public string Path { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [OpenApiSubTag("List", "Detail")]
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
        [OpenApiSubTag("List", "Detail")]
        [OpenApiSchema(OpenApiSchemaType.@string, OpenApiSchemaFormat.string_datetime)]
        [JsonConverter(typeof(DateTimeConverter), "yyyy-MM-dd HH:mm:ss")]
        public DateTime CreateTime { get; set; }
    }
}

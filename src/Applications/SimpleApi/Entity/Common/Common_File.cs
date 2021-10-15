using FreeSql.DataAnnotations;
using Microservice.Library.OpenApi.JsonExtension;
using Microservice.Library.OpenApi.Annotations;
using Newtonsoft.Json;
using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Entity.Common
{
    /// <summary>
    /// 文件信息
    /// </summary>
    [Table]
    [OraclePrimaryKeyName("pk_C_F")]
    [Index("C_F_idx_01", nameof(Name) + " ASC")]
    [Index("C_F_idx_02", nameof(FileType) + " ASC")]
    [Index("C_F_idx_03", nameof(ContentType) + " ASC")]
    [Index("C_F_idx_04", nameof(Extension) + " ASC")]
    [Index("C_F_idx_05", nameof(ServerKey) + " ASC")]
    [Index("C_F_idx_06", nameof(MD5) + " ASC")]
    [Index("C_F_idx_08", nameof(CreateTime) + " DESC")]
    public class Common_File
    {
        /// <summary>
        /// Id
        /// </summary>
        [OpenApiSubTag("List", "Detail", "FileInfo")]
        [Column(IsPrimary = true, StringLength = 36)]
        public string Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [OpenApiSubTag("List", "Detail", "FileInfo")]
        [Column(StringLength = 256)]
        public string Name { get; set; }

        /// <summary>
        /// 文件类型
        /// <para>电子文档</para>
        /// <para>电子表格</para>
        /// <para>图片</para>
        /// <para>音频</para>
        /// <para>视频</para>
        /// <para>压缩包</para>
        /// <para>外链资源</para>
        /// </summary>
        [OpenApiSubTag("List", "Detail", "FileInfo")]
        [Column(StringLength = 10)]
        public string FileType { get; set; }

        /// <summary>
        /// 内容类型
        /// </summary>
        [OpenApiSubTag("List", "Detail", "FileInfo")]
        [Column(StringLength = 100)]
        public string ContentType { get; set; }

        /// <summary>
        /// 文件扩展名
        /// </summary>
        [OpenApiSubTag("List", "Detail", "FileInfo")]
        [Column(StringLength = 30)]
        public string Extension { get; set; }

        /// <summary>
        /// MD5校验值
        /// </summary>
        [OpenApiSubTag("List", "Detail", "FileInfo")]
        [Column(StringLength = 36)]
        public string MD5 { get; set; }

        /// <summary>
        /// 服务器标识
        /// </summary>
        [OpenApiSubTag("List", "Detail", "FileInfo")]
        [Column(StringLength = 36)]
        public string ServerKey { get; set; }

        /// <summary>
        /// 存储类型
        /// <para>Path</para>
        /// <para>Uri</para>
        /// <para>Base64</para>
        /// <para>Base64Url</para>
        /// </summary>
        [OpenApiSubTag("List", "Detail", "FileInfo")]
        [Column(StringLength = 10)]
        public string StorageType { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        [OpenApiSubTag("_List")]
        [Column(StringLength = -1)]
        public string Path { get; set; }

        /// <summary>
        /// 字节数
        /// </summary>
        [OpenApiSubTag("_List", "Detail")]
        [Column(IsNullable = true)]
        public long? Bytes { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        [OpenApiSubTag("List", "Detail", "FileInfo")]
        [Column(StringLength = 100, IsNullable = true)]
        public string Size { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [OpenApiSubTag("List", "Detail", "FileInfo")]
        [Column(StringLength = 10)]
        public string State { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [OpenApiSubTag("List", "Detail", "FileInfo")]
        [OpenApiSchema(OpenApiSchemaType.@string, OpenApiSchemaFormat.string_datetime)]
        [JsonConverter(typeof(DateTimeConverter), "yyyy-MM-dd HH:mm:ss")]
        public DateTime CreateTime { get; set; }

        #region 关联

        /// <summary>
        /// 相关个人文件信息
        /// </summary>
        [Navigate(nameof(Common_PersonalFileInfo.FileId))]
        [OpenApiIgnore]
        [JsonIgnore]
        [XmlIgnore]
        public virtual ICollection<Common_PersonalFileInfo> PersonalFileInfos { get; set; }

        #endregion
    }
}

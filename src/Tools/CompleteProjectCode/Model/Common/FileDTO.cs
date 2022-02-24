using Entity.Common;
using Microservice.Library.DataMapping.Annotations;
using Microservice.Library.DataMapping.Application;
using Microservice.Library.OpenApi.Annotations;
using Newtonsoft.Json;
using System.Xml.Serialization;

/* 
 * 文件信息业务模型
 */
namespace Model.Common.FileDTO
{
    /// <summary>
    /// 文件信息
    /// </summary>
    [MapFrom(typeof(Common_File))]
    [MapTo(typeof(Common_File))]
    [OpenApiMainTag("FileInfo")]
    public class FileInfo : Common_File
    {
        /// <summary>
        /// 来源成员映射选项
        /// </summary>
        [OpenApiIgnore]
        public static MemberMapOptions<Common_File, FileInfo> FromMemberMapOptions =
            new MemberMapOptions<Common_File, FileInfo>();

        /// <summary>
        /// 当前成员映射选项
        /// </summary>
        [OpenApiIgnore]
        public static MemberMapOptions<FileInfo, Common_File> ToMemberMapOptions =
            new MemberMapOptions<FileInfo, Common_File>();
    }

    /// <summary>
    /// 文件库信息
    /// </summary>
    public class LibraryInfo
    {
        /// <summary>
        /// 文件类型
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// 文件总数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 文件总字节数
        /// </summary>
        [OpenApiIgnore]
        [JsonIgnore]
        [XmlIgnore]
        public long _Bytes { get; set; }

        /// <summary>
        /// 文件总字节数
        /// </summary>
        public string Bytes { get; set; }

        /// <summary>
        /// 文件占用存储空间
        /// </summary>
        public string Size { get; set; }
    }
}

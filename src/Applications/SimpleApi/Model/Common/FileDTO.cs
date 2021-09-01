using Entity.Common;
using Microservice.Library.DataMapping.Annotations;
using Microservice.Library.DataMapping.Application;
using Microservice.Library.OpenApi.Annotations;

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
}

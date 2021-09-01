using Entity.Common;
using Microservice.Library.DataMapping.Annotations;
using Microservice.Library.DataMapping.Application;
using Microservice.Library.OpenApi.Annotations;

/* 
 * 个人文件信息业务模型
 */
namespace Model.Common.PersonalFileInfoDTO
{
    /// <summary>
    /// 文件信息
    /// </summary>
    [MapFrom(typeof(Common_PersonalFileInfo))]
    [MapTo(typeof(Common_PersonalFileInfo))]
    [OpenApiMainTag("PersonalFileInfo")]
    public class PersonalFileInfo : Common_PersonalFileInfo
    {
        /// <summary>
        /// 来源成员映射选项
        /// </summary>
        [OpenApiIgnore]
        public static MemberMapOptions<Common_PersonalFileInfo, PersonalFileInfo> FromMemberMapOptions =
            new MemberMapOptions<Common_PersonalFileInfo, PersonalFileInfo>();

        /// <summary>
        /// 当前成员映射选项
        /// </summary>
        [OpenApiIgnore]
        public static MemberMapOptions<PersonalFileInfo, Common_PersonalFileInfo> ToMemberMapOptions =
            new MemberMapOptions<PersonalFileInfo, Common_PersonalFileInfo>();
    }

    /// <summary>
    /// 编辑
    /// </summary>
    [MapFrom(typeof(Common_PersonalFileInfo))]
    [OpenApiMainTag("Edit")]
    public class Edit : Common_PersonalFileInfo
    {

    }
}

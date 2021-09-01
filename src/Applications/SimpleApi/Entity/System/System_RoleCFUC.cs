using Entity.Common;
using FreeSql.DataAnnotations;
using Microservice.Library.OpenApi.Annotations;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace Entity.System
{
    /// <summary>
    /// 角色授权文件上传配置
    /// </summary>
    [Table]
    [OraclePrimaryKeyName("pk1_S_RCFUC,pk2_S_RCFUC")]
    public class System_RoleCFUC
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        [Column(IsPrimary = true, StringLength = 36)]
        public string RoleId { get; set; }

        /// <summary>
        /// 配置Id
        /// </summary>
        [Column(IsPrimary = true, StringLength = 36)]
        public string ConfigId { get; set; }

        #region 关联

        /// <summary>
        /// 角色
        /// </summary>
        [Navigate(nameof(RoleId))]
        [OpenApiIgnore]
        [JsonIgnore]
        [XmlIgnore]
        public virtual System_Role Role { get; set; }

        /// <summary>
        /// 文件上传配置
        /// </summary>
        [Navigate(nameof(ConfigId))]
        [OpenApiIgnore]
        [JsonIgnore]
        [XmlIgnore]
        public virtual Common_FileUploadConfig FileUploadConfig { get; set; }

        #endregion
    }
}

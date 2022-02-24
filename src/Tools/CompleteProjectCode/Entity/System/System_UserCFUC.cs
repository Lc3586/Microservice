using Entity.Common;
using FreeSql.DataAnnotations;
using Microservice.Library.OpenApi.Annotations;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace Entity.System
{
    /// <summary>
    /// 用户授权文件上传配置
    /// </summary>
    [Table]
    [OraclePrimaryKeyName("pk1_S_UCFUC,pk2_S_UCFUC")]
    public class System_UserCFUC
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        [Column(IsPrimary = true, StringLength = 36)]
        public string UserId { get; set; }

        /// <summary>
        /// 配置Id
        /// </summary>
        [Column(IsPrimary = true, StringLength = 36)]
        public string ConfigId { get; set; }

        #region 关联

        /// <summary>
        /// 用户
        /// </summary>
        [Navigate(nameof(UserId))]
        [OpenApiIgnore]
        [JsonIgnore]
        [XmlIgnore]
        public virtual System_User User { get; set; }

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

using FreeSql.DataAnnotations;
using Microservice.Library.OpenApi.Annotations;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace Entity.System
{
    /// <summary>
    /// 角色授权资源
    /// </summary>
    [Table]
    [OraclePrimaryKeyName("pk1_S_RR,pk2_S_RR")]
    public class System_RoleResources
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        [Column(IsPrimary = true, StringLength = 36)]
        public string RoleId { get; set; }

        /// <summary>
        /// 资源Id
        /// </summary>
        [Column(IsPrimary = true, StringLength = 36)]
        public string ResourcesId { get; set; }

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
        /// 资源
        /// </summary>
        [Navigate(nameof(ResourcesId))]
        [OpenApiIgnore]
        [JsonIgnore]
        [XmlIgnore]
        public virtual System_Resources Resources { get; set; }

        #endregion
    }
}

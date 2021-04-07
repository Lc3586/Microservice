using FreeSql.DataAnnotations;
using Microservice.Library.OpenApi.Annotations;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace Entity.System
{
    /// <summary>
    /// 菜单相关资源
    /// </summary>
    [Table]
    public class System_MenuResources
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        [Column(IsPrimary = true, StringLength = 36)]
        public string MenuId { get; set; }

        /// <summary>
        /// 资源Id
        /// </summary>
        [Column(IsPrimary = true, StringLength = 36)]
        public string ResourcesId { get; set; }

        #region 关联

        /// <summary>
        /// 菜单
        /// </summary>
        [Navigate(nameof(MenuId))]
        [OpenApiIgnore]
        [JsonIgnore]
        [XmlIgnore]
        public virtual System_Menu Menu { get; set; }

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

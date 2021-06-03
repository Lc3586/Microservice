using FreeSql.DataAnnotations;
using Microservice.Library.OpenApi.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Entity.System
{
    /// <summary>
    /// 菜单
    /// </summary>
    [Table]
    [OraclePrimaryKeyName("pk_S_M")]
    #region 设置索引
    [Index("S_M_idx_01", nameof(RootId) + " ASC")]
    [Index("S_M_idx_02", nameof(ParentId) + " ASC")]
    [Index("S_M_idx_03", nameof(Name) + " ASC")]
    [Index("S_M_idx_04", nameof(Type) + " ASC")]
    [Index("S_M_idx_05", nameof(Code) + " ASC")]
    [Index("S_M_idx_06", nameof(Uri) + " ASC")]
    [Index("S_M_idx_07", nameof(Method) + " ASC")]
    [Index("S_M_idx_08", nameof(Enable) + " DESC")]
    [Index("S_M_idx_09", nameof(CreatorId) + " ASC")]
    [Index("S_M_idx_10", nameof(CreateTime) + " DESC")]
    [Index("S_M_idx_11", nameof(ModifyTime) + " DESC")]
    #endregion
    public class System_Menu
    {
        /// <summary>
        /// Id
        /// </summary>
        [OpenApiSubTag("List", "TreeList", "Edit", "Detail", "Authorities", "Sort")]
        [Column(IsPrimary = true, StringLength = 36)]
        public string Id { get; set; }

        /// <summary>
        /// 根菜单Id
        /// </summary>
        [OpenApiSubTag("List", "TreeList", "_Edit", "Sort")]
        [Column(StringLength = 36)]
        public string RootId { get; set; }

        /// <summary>
        /// 父级菜单Id
        /// </summary>
        [OpenApiSubTag("List", "TreeList", "Create", "Edit", "Sort")]
        [Column(StringLength = 36)]
        public string ParentId { get; set; }

        /// <summary>
        /// 层级
        /// </summary>
        [OpenApiSubTag("List", "TreeList", "_Edit", "Sort")]
        public int Level { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [OpenApiSubTag("List", "TreeList", "Create", "Edit", "Detail", "Authorities")]
        [Description("名称")]
        [Column(StringLength = 50)]
        public string Name { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [OpenApiSubTag("List", "TreeList", "Create", "Edit", "Detail", "Authorities")]
        [Description("类型")]
        [Column(StringLength = 20)]
        public string Type { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        [OpenApiSubTag("List", "TreeList", "Create", "Edit", "Detail", "Authorities")]
        [Description("编码")]
        [Column(StringLength = 36)]
        public string Code { get; set; }

        /// <summary>
        /// 链接地址
        /// </summary>
        [OpenApiSubTag("List", "TreeList", "Create", "Edit", "Detail", "Authorities")]
        [Description("链接地址")]
        [Column(StringLength = 2000)]
        public string Uri { get; set; }

        /// <summary>
        /// 请求方法
        /// </summary>
        [OpenApiSubTag("List", "TreeList", "Create", "Edit", "Detail", "Authorities")]
        [Description("请求方法")]
        [Column(StringLength = 10)]
        public string Method { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        [OpenApiSubTag("List", "TreeList", "Create", "Edit", "Detail")]
        [Description("图标")]
        [Column(StringLength = 36)]
        public string Icon { get; set; }

        /// <summary>
        /// 启用缓存
        /// </summary>
        [OpenApiSubTag("List", "TreeList", "Create", "Edit", "Detail")]
        [Description("启用缓存")]
        public bool Cache { get; set; }

        /// <summary>
        /// 启用
        /// </summary>
        [OpenApiSubTag("List", "TreeList", "Create", "Edit", "Detail")]
        [Description("启用")]
        public bool Enable { get; set; }

        /// <summary>
        /// 排序值
        /// </summary>
        [OpenApiSubTag("List", "TreeList", "Sort")]
        [Description("排序值")]
        public int Sort { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [OpenApiSubTag("Create", "Edit", "Detail")]
        [Description("备注")]
        [Column(StringLength = -2)]
        public string Remark { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        [Column(StringLength = 36)]
        public string CreatorId { get; set; }

        /// <summary>
        /// 创建者名称
        /// </summary>
        [OpenApiSubTag("List", "TreeList", "Detail")]
        [OpenApiSchema(OpenApiSchemaType.@string)]
        [Description("创建者")]
        [Column(StringLength = 50)]
        public string CreatorName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [OpenApiSubTag("List", "TreeList", "Detail")]
        [OpenApiSchema(OpenApiSchemaType.@string, OpenApiSchemaFormat.string_datetime)]
        [JsonConverter(typeof(Microservice.Library.OpenApi.JsonExtension.DateTimeConverter), "yyyy-MM-dd HH:mm:ss")]
        [Description("创建时间")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 最近编辑者
        /// </summary>
        [OpenApiSubTag("_Edit")]
        [Column(StringLength = 36)]
        public string ModifiedById { get; set; }

        /// <summary>
        /// 最近编辑者名称
        /// </summary>
        [OpenApiSubTag("List", "Detail", "_Edit")]
        [Description("最近编辑者")]
        [Column(StringLength = 50)]
        public string ModifiedByName { get; set; }

        /// <summary>
        /// 最近编辑时间
        /// </summary>
        [OpenApiSubTag("List", "Detail", "_Edit")]
        [OpenApiSchema(OpenApiSchemaType.@string, OpenApiSchemaFormat.string_datetime)]
        [JsonConverter(typeof(Microservice.Library.OpenApi.JsonExtension.DateTimeConverter), "yyyy-MM-dd HH:mm:ss")]
        [Description("最近编辑时间")]
        [Column(IsNullable = true)]
        public DateTime? ModifyTime { get; set; }

        #region 关联

        /// <summary>
        /// 根菜单
        /// </summary>
        [Navigate(nameof(RootId))]
        [OpenApiIgnore]
        [JsonIgnore]
        [XmlIgnore]
        public virtual System_Menu Root { get; set; }

        /// <summary>
        /// 父级菜单
        /// </summary>
        [Navigate(nameof(ParentId))]
        [OpenApiIgnore]
        [JsonIgnore]
        [XmlIgnore]
        public virtual System_Menu Parent { get; set; }

        /// <summary>
        /// 子菜单
        /// </summary>
        [Navigate(nameof(ParentId))]
        [OpenApiIgnore]
        [JsonIgnore]
        [XmlIgnore]
        public virtual ICollection<System_Menu> Childs { get; set; }

        /// <summary>
        /// 被授权此菜单的用户
        /// </summary>
        [Navigate(ManyToMany = typeof(System_UserMenu))]
        [OpenApiIgnore]
        [JsonIgnore]
        [XmlIgnore]
        public virtual ICollection<System_User> Users { get; set; }

        /// <summary>
        /// 被授权此菜单的角色
        /// </summary>
        [Navigate(ManyToMany = typeof(System_RoleMenu))]
        [OpenApiIgnore]
        [JsonIgnore]
        [XmlIgnore]
        public virtual ICollection<System_Role> Roles { get; set; }

        /// <summary>
        /// 此菜单关联的资源
        /// </summary>
        [Navigate(ManyToMany = typeof(System_MenuResources))]
        [OpenApiIgnore]
        [JsonIgnore]
        [XmlIgnore]
        public virtual ICollection<System_Resources> Resources { get; set; }

        #endregion
    }
}

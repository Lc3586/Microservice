﻿using Entity.Common;
using FreeSql.DataAnnotations;
using Microservice.Library.OpenApi.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Entity.System
{
    /// <summary>
    /// 系统用户
    /// </summary>
    [Table]
    [OraclePrimaryKeyName("pk_S_U")]
    #region 设置索引
    [Index("S_U_idx_01", nameof(Account) + " ASC")]
    [Index("S_U_idx_02", nameof(Nickname) + " ASC")]
    [Index("S_U_idx_03", nameof(Name) + " ASC")]
    [Index("S_U_idx_04", nameof(Tel) + " ASC")]
    [Index("S_U_idx_05", nameof(Enable) + " DESC")]
    [Index("S_U_idx_06", nameof(CreatorId) + " ASC")]
    [Index("S_U_idx_07", nameof(CreateTime) + " DESC")]
    [Index("S_U_idx_08", nameof(ModifyTime) + " DESC")]
    #endregion
    public class System_User
    {
        /// <summary>
        /// Id
        /// </summary>
        [OpenApiSubTag("List", "Edit", "Detail", "Authorities", "UpdatePassword")]
        [Column(IsPrimary = true, StringLength = 36)]
        public string Id { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        [OpenApiSubTag("List", "Create", "Detail", "Authorities", "Login")]
        [Required(ErrorMessage = "账号不可为空")]
        [Description("账号")]
        [Column(StringLength = 50)]
        public string Account { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [OpenApiSubTag("Create", "_Edit", "Login")]
        [Required(ErrorMessage = "密码不可为空")]
        [Description("密码")]
        [Column(StringLength = 50)]
        public string Password { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [OpenApiSubTag("List", "Create", "Edit", "Detail")]
        [Description("昵称")]
        [Column(StringLength = 50)]
        public string Nickname { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [OpenApiSubTag("List", "Create", "Edit", "Detail")]
        [Description("姓名")]
        [Column(StringLength = 20)]
        public string Name { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [OpenApiSubTag("List", "Create", "Edit", "Detail")]
        [Description("性别")]
        [Column(StringLength = 2)]
        public string Sex { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        [OpenApiSubTag("List", "Create", "Edit", "Detail")]
        [Description("手机号码")]
        [Column(StringLength = 20)]
        public string Tel { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        [OpenApiSubTag("List", "Create", "Edit", "Detail")]
        [Description("头像")]
        [Column(StringLength = 36)]
        public string Face { get; set; }

        /// <summary>
        /// 启用
        /// </summary>
        [OpenApiSubTag("List", "Create", "Edit", "Detail", "Authorities")]
        [Description("启用")]
        public bool Enable { get; set; }

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
        [OpenApiSubTag("List", "Detail")]
        [OpenApiSchema(OpenApiSchemaType.@string)]
        [Description("创建者")]
        [Column(StringLength = 50)]
        public string CreatorName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [OpenApiSubTag("List", "Detail")]
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
        /// 头像文件
        /// </summary>
        [Navigate(nameof(Face))]
        [OpenApiIgnore]
        [JsonIgnore]
        [XmlIgnore]
        public virtual Common_File FaceFile { get; set; }

        /// <summary>
        /// 授权给此用户的角色
        /// </summary>
        [Navigate(ManyToMany = typeof(System_UserRole))]
        [OpenApiIgnore]
        [JsonIgnore]
        [XmlIgnore]
        public virtual ICollection<System_Role> Roles { get; set; }

        /// <summary>
        /// 直接授权给此用户的菜单
        /// </summary>
        [Navigate(ManyToMany = typeof(System_UserMenu))]
        [OpenApiIgnore]
        [JsonIgnore]
        [XmlIgnore]
        public virtual ICollection<System_Menu> Menus { get; set; }

        /// <summary>
        /// 直接授权给此用户的资源
        /// </summary>
        [Navigate(ManyToMany = typeof(System_UserResources))]
        [OpenApiIgnore]
        [JsonIgnore]
        [XmlIgnore]
        public virtual ICollection<System_Resources> Resources { get; set; }

        /// <summary>
        /// 直接授权给此用户的文件上传配置
        /// </summary>
        [Navigate(ManyToMany = typeof(System_UserCFUC))]
        [OpenApiIgnore]
        [JsonIgnore]
        [XmlIgnore]
        public virtual ICollection<Common_FileUploadConfig> FileUploadConfigs { get; set; }

        /// <summary>
        /// 此用户绑定的微信
        /// </summary>
        [Navigate(ManyToMany = typeof(System_UserWeChatUserInfo))]
        [OpenApiIgnore]
        [JsonIgnore]
        [XmlIgnore]
        public virtual ICollection<Common_WeChatUserInfo> WeChatUserInfos { get; set; }

        /// <summary>
        /// 上传的个人文件信息
        /// </summary>
        [JsonIgnore]
        [Navigate(nameof(Common_PersonalFileInfo.CreatorId))]
        [OpenApiIgnore]
        [XmlIgnore]
        public virtual ICollection<Common_PersonalFileInfo> PersonalFileInfos { get; set; }

        #endregion
    }
}

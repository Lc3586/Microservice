using Entity.System;
using Microservice.Library.DataMapping.Annotations;
using Microservice.Library.OpenApi.Annotations;
using System.Collections.Generic;
using System.ComponentModel;

/* 
 * 角色业务模型
 */
namespace Model.System.RoleDTO
{
    /// <summary>
    /// 列表
    /// </summary>
    [MapFrom(typeof(System_Role))]
    [OpenApiMainTag("List")]
    public class List : System_Role
    {

    }

    /// <summary>
    /// 树状列表参数
    /// </summary>
    public class TreeListParamter
    {
        /// <summary>
        /// 父级Id
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 层级数
        /// <para>为空则表示获取所有层级数据</para>
        /// </summary>
        public int? Rank { get; set; }

        /// <summary>
        /// 角色类型
        /// </summary>
        public List<string> RoleType { get; set; } = new List<string>();
    }

    /// <summary>
    /// 树状列表
    /// </summary>
    [MapFrom(typeof(System_Role))]
    [OpenApiMainTag("TreeList")]
    public class TreeList : System_Role
    {
        /// <summary>
        /// 是否拥有子级
        /// </summary>
        public bool HasChildren { get; set; }

        /// <summary>
        /// 子级数量
        /// </summary>
        public int ChildrenCount { get; set; }

        /// <summary>
        /// 子级角色
        /// </summary>
        [OpenApiSchema(OpenApiSchemaType.model, OpenApiSchemaFormat.model_once)]
        [Description("子级角色")]
        public List<TreeList> Children { get; set; }
    }

    /// <summary>
    /// 详情
    /// </summary>
    [MapFrom(typeof(System_Role))]
    [OpenApiMainTag("Detail")]
    public class Detail : System_Role
    {

    }

    /// <summary>
    /// 授权信息树状列表
    /// </summary>
    [MapFrom(typeof(System_Role))]
    [OpenApiMainTag("Authorities")]
    public class AuthoritiesTree : System_Role
    {
        /// <summary>
        /// 已授权
        /// </summary>
        public bool Authorized { get; set; }

        /// <summary>
        /// 是否拥有子级
        /// </summary>
        public bool HasChildren { get; set; }

        /// <summary>
        /// 子级数量
        /// </summary>
        public int ChildrenCount { get; set; }

        /// <summary>
        /// 子级授权信息
        /// </summary>
        [OpenApiSchema(OpenApiSchemaType.model, OpenApiSchemaFormat.model_once)]
        [Description("子级授权信息")]
        public List<AuthoritiesTree> Children { get; set; }
    }

    /// <summary>
    /// 授权信息
    /// </summary>
    [MapFrom(typeof(System_Role))]
    [OpenApiMainTag("Authorities")]
    public class Authorities : System_Role
    {
        /// <summary>
        /// 授权给此角色的菜单
        /// </summary>
        [OpenApiSchema(OpenApiSchemaType.model)]
        [Description("授权给此角色的菜单")]
        public List<MenuDTO.Authorities> _Menus { get; set; }

        /// <summary>
        /// 授权给此角色的资源
        /// </summary>
        [OpenApiSchema(OpenApiSchemaType.model)]
        [Description("授权给此角色的资源")]
        public List<ResourcesDTO.Authorities> _Resources { get; set; }

        /// <summary>
        /// 授权给此角色的文件上传配置
        /// </summary>
        [OpenApiSchema(OpenApiSchemaType.model)]
        [Description("授权给此角色的文件上传配置")]
        public List<Common.FileUploadConfigDTO.Authorities> _FileUploadConfigs { get; set; }
    }

    /// <summary>
    /// 新增
    /// </summary>
    [MapTo(typeof(System_Role))]
    [OpenApiMainTag("Create")]
    public class Create : System_Role
    {

    }

    /// <summary>
    /// 编辑
    /// </summary>
    [MapFrom(typeof(System_Role))]
    [MapTo(typeof(System_Role))]
    [OpenApiMainTag("Edit")]
    public class Edit : System_Role
    {

    }

    /// <summary>
    /// 授权角色给用户
    /// </summary>
    public class AuthorizeToUser
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 用户Id集合
        /// </summary>
        public List<string> UserIds { get; set; }
    }

    /// <summary>
    /// 授权菜单给角色
    /// </summary>
    public class AuthorizeFromMenu
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 菜单Id集合
        /// </summary>
        public List<string> MenusIds { get; set; }
    }

    /// <summary>
    /// 授权资源给角色
    /// </summary>
    public class AuthorizeFromResources
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 资源Id集合
        /// </summary>
        public List<string> ResourcesIds { get; set; }
    }
}

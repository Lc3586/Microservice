using Entity.System;
using Microservice.Library.DataMapping.Annotations;
using Microservice.Library.OpenApi.Annotations;
using System.Collections.Generic;
using System.ComponentModel;

/* 
 * 菜单业务模型
 */
namespace Model.System.MenuDTO
{
    /// <summary>
    /// 列表
    /// </summary>
    [MapFrom(typeof(System_Menu))]
    [OpenApiMainTag("List")]
    public class List : System_Menu
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
        /// 菜单类型
        /// </summary>
        public List<string> MenuType { get; set; } = new List<string>();
    }

    /// <summary>
    /// 树状列表
    /// </summary>
    [MapFrom(typeof(System_Menu))]
    [OpenApiMainTag("TreeList")]
    public class TreeList : System_Menu
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
        /// 子级菜单
        /// </summary>
        [OpenApiSchema(OpenApiSchemaType.model, OpenApiSchemaFormat.model_once)]
        [Description("子级菜单")]
        public List<TreeList> Children { get; set; }
    }

    /// <summary>
    /// 详情
    /// </summary>
    [MapFrom(typeof(System_Menu))]
    [OpenApiMainTag("Detail")]
    public class Detail : System_Menu
    {

    }

    /// <summary>
    /// 授权信息树状列表
    /// </summary>
    [MapFrom(typeof(System_Menu))]
    [OpenApiMainTag("Authorities")]
    public class AuthoritiesTree : System_Menu
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
    [MapFrom(typeof(System_Menu))]
    [OpenApiMainTag("Authorities")]
    public class Authorities : System_Menu
    {

    }

    /// <summary>
    /// 新增
    /// </summary>
    [MapTo(typeof(System_Menu))]
    [OpenApiMainTag("Create")]
    public class Create : System_Menu
    {

    }

    /// <summary>
    /// 编辑
    /// </summary>
    [MapFrom(typeof(System_Menu))]
    [MapTo(typeof(System_Menu))]
    [OpenApiMainTag("Edit")]
    public class Edit : System_Menu
    {

    }
}

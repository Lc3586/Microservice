using Model.Utils.Pagination;
using System.Collections.Generic;

namespace Business.Interface.System
{
    /// <summary>
    /// 权限业务接口类
    /// </summary>
    public interface IAuthoritiesBusiness
    {
        #region 授权

        /// <summary>
        /// 自动授权角色给用户
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        void AutoAuthorizeRoleForUser(Model.System.AuthorizeDTO.RoleForUser data, bool runTransaction = true);

        /// <summary>
        /// 自动授权角色给会员
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        void AutoAuthorizeRoleForMember(Model.System.AuthorizeDTO.RoleForMember data, bool runTransaction = true);

        /// <summary>
        /// 授权角色给用户
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        void AuthorizeRoleForUser(Model.System.AuthorizeDTO.RoleForUser data, bool runTransaction = true);

        /// <summary>
        /// 授权角色给用户
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="allChilds">同时授权所有子集角色</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        void AuthorizeRoleForUser(string roleId, string userId, bool allChilds = false, bool runTransaction = true);

        /// <summary>
        /// 授权角色给会员
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        void AuthorizeRoleForMember(Model.System.AuthorizeDTO.RoleForMember data, bool runTransaction = true);

        /// <summary>
        /// 授权角色给会员
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="memberId">会员Id</param>
        /// <param name="allChilds">同时授权所有子集角色</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        void AuthorizeRoleForMember(string roleId, string memberId, bool allChilds = false, bool runTransaction = true);

        /// <summary>
        /// 直接授权菜单给用户
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        void AuthorizeMenuForUser(Model.System.AuthorizeDTO.MenuForUser data, bool runTransaction = true);

        /// <summary>
        /// 直接授权菜单给用户
        /// </summary>
        /// <param name="menuId">菜单Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="allChilds">同时授权所有子集菜单</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        void AuthorizeMenuForUser(string menuId, string userId, bool allChilds = false, bool runTransaction = true);

        /// <summary>
        /// 直接授权资源给用户
        /// </summary>
        /// <param name="data">数据</param>
        void AuthorizeResourcesForUser(Model.System.AuthorizeDTO.ResourcesForUser data);

        /// <summary>
        /// 授权菜单给角色
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        void AuthorizeMenuForRole(Model.System.AuthorizeDTO.MenuForRole data, bool runTransaction = true);

        /// <summary>
        /// 授权菜单给角色
        /// </summary>
        /// <param name="menuId">菜单Id</param>
        /// <param name="roleId">角色Id</param>
        /// <param name="allChilds">同时授权所有子集菜单</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        void AuthorizeMenuForRole(string menuId, string roleId, bool allChilds = false, bool runTransaction = true);

        /// <summary>
        /// 授权资源给角色
        /// </summary>
        /// <param name="data">数据</param>
        void AuthorizeResourcesForRole(Model.System.AuthorizeDTO.ResourcesForRole data);

        /// <summary>
        /// 直接授权文件上传配置给用户
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        void AuthorizeCFUCForUser(Model.System.AuthorizeDTO.CFUCForUser data, bool runTransaction = true);

        /// <summary>
        /// 直接授权文件上传配置给用户
        /// </summary>
        /// <param name="configId">文件上传配置Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="allChilds">同时授权所有子集菜单</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        void AuthorizeCFUCForUser(string configId, string userId, bool allChilds = false, bool runTransaction = true);

        /// <summary>
        /// 授权文件上传配置给角色
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        void AuthorizeCFUCForRole(Model.System.AuthorizeDTO.CFUCForRole data, bool runTransaction = true);

        /// <summary>
        /// 授权文件上传配置给角色
        /// </summary>
        /// <param name="configId">文件上传配置Id</param>
        /// <param name="roleId">角色Id</param>
        /// <param name="allChilds">同时授权所有子集菜单</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        void AuthorizeCFUCForRole(string configId, string roleId, bool allChilds = false, bool runTransaction = true);

        #endregion

        #region 撤销授权

        /// <summary>
        /// 撤销角色的全部系统用户授权
        /// </summary>
        /// <param name="roleIds">角色Id</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        void RevocationRoleForAllUser(List<string> roleIds, bool runTransaction = true);

        /// <summary>
        /// 撤销角色的全部会员授权
        /// </summary>
        /// <param name="roleIds">角色Id</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        void RevocationRoleForAllMember(List<string> roleIds, bool runTransaction = true);

        /// <summary>
        /// 撤销用户的全部角色授权
        /// </summary>
        /// <param name="userIds">用户Id</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        void RevocationRoleForUser(List<string> userIds, bool runTransaction = true);

        /// <summary>
        /// 撤销会员的全部角色授权
        /// </summary>
        /// <param name="memberIds">会员Id</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        void RevocationRoleForMember(List<string> memberIds, bool runTransaction = true);

        /// <summary>
        /// 撤销用户的全部菜单授权
        /// </summary>
        /// <param name="userIds">用户Id</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        void RevocationMenuForUser(List<string> userIds, bool runTransaction = true);

        /// <summary>
        /// 撤销用户的全部资源授权
        /// </summary>
        /// <param name="userIds">用户Id</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        void RevocationResourcesForUser(List<string> userIds, bool runTransaction = true);

        /// <summary>
        /// 撤销用户的全部文件上传配置授权
        /// </summary>
        /// <param name="userIds">用户Id</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        void RevocationCFUCForUser(List<string> userIds, bool runTransaction = true);

        /// <summary>
        /// 撤销角色的全部菜单授权
        /// </summary>
        /// <param name="roleIds">角色Id</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        void RevocationMenuForRole(List<string> roleIds, bool runTransaction = true);

        /// <summary>
        /// 撤销角色的全部资源授权
        /// </summary>
        /// <param name="roleIds">角色Id</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        void RevocationResourcesForRole(List<string> roleIds, bool runTransaction = true);

        /// <summary>
        /// 撤销角色的全部文件上传配置授权
        /// </summary>
        /// <param name="roleIds">角色Id</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        void RevocationCFUCForRole(List<string> roleIds, bool runTransaction = true);

        /// <summary>
        /// 撤销用户的角色授权
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        void RevocationRoleForUser(Model.System.AuthorizeDTO.RoleForUser data, bool runTransaction = true);

        /// <summary>
        /// 撤销用户的角色授权
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="allChilds">同时撤销所有子集角色授权</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        void RevocationRoleForUser(string roleId, string userId, bool allChilds = false, bool runTransaction = true);

        /// <summary>
        /// 撤销会员的角色授权
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        void RevocationRoleForMember(Model.System.AuthorizeDTO.RoleForMember data, bool runTransaction = true);

        /// <summary>
        /// 撤销会员的角色授权
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="memberId">会员Id</param>
        /// <param name="allChilds">同时撤销所有子集角色授权</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        void RevocationRoleForMember(string roleId, string memberId, bool allChilds = false, bool runTransaction = true);

        /// <summary>
        /// 撤销用户的直接菜单授权
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        void RevocationMenuForUser(Model.System.AuthorizeDTO.MenuForUser data, bool runTransaction = true);

        /// <summary>
        /// 撤销用户的直接菜单授权
        /// </summary>
        /// <param name="menuId">菜单Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="allChilds">同时撤销所有子集菜单授权</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        void RevocationMenuForUser(string menuId, string userId, bool allChilds = false, bool runTransaction = true);

        /// <summary>
        /// 撤销用户的直接资源授权
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        void RevocationResourcesForUser(Model.System.AuthorizeDTO.ResourcesForUser data, bool runTransaction = true);

        /// <summary>
        /// 撤销用户的直接文件上传配置授权
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        void RevocationCFUCForUser(Model.System.AuthorizeDTO.CFUCForUser data, bool runTransaction = true);

        /// <summary>
        /// 撤销用户的直接文件上传配置授权
        /// </summary>
        /// <param name="configId">文件上传配置Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="allChilds">同时撤销所有子集文件上传配置授权</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        void RevocationCFUCForUser(string configId, string userId, bool allChilds = false, bool runTransaction = true);

        /// <summary>
        /// 撤销角色的菜单授权
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        void RevocationMenuForRole(Model.System.AuthorizeDTO.MenuForRole data, bool runTransaction = true);

        /// <summary>
        /// 撤销角色的菜单授权
        /// </summary>
        /// <param name="menuId">菜单Id</param>
        /// <param name="roleId">角色Id</param>
        /// <param name="allChilds">同时撤销所有子集菜单授权</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        void RevocationMenuForRole(string menuId, string roleId, bool allChilds = false, bool runTransaction = true);

        /// <summary>
        /// 撤销角色的资源授权
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        void RevocationResourcesForRole(Model.System.AuthorizeDTO.ResourcesForRole data, bool runTransaction = true);

        /// <summary>
        /// 撤销角色的文件上配置授权
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        void RevocationCFUCForRole(Model.System.AuthorizeDTO.CFUCForRole data, bool runTransaction = true);

        /// <summary>
        /// 撤销角色的文件上配置授权
        /// </summary>
        /// <param name="configId">文件上配置Id</param>
        /// <param name="roleId">角色Id</param>
        /// <param name="allChilds">同时撤销所有子集文件上配置授权</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        void RevocationCFUCForRole(string configId, string roleId, bool allChilds = false, bool runTransaction = true);

        /// <summary>
        /// 撤销所有用户和角色的菜单授权
        /// </summary>
        /// <param name="menuIds">菜单Id</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        void RevocationMenuForAll(List<string> menuIds, bool runTransaction = true);

        /// <summary>
        /// 撤销所有用户和角色的资源授权
        /// </summary>
        /// <param name="resourcesIds">资源Id</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        void RevocationResourcesForAll(List<string> resourcesIds, bool runTransaction = true);

        /// <summary>
        /// 撤销所有用户和角色的文件上配置授权
        /// </summary>
        /// <param name="configIds">文件上配置Id</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        void RevocationCFUCForAll(List<string> configIds, bool runTransaction = true);

        #endregion

        #region 获取授权

        /// <summary>
        /// 获取用户的授权数据
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="includeRole">包括授权角色</param>
        /// <param name="includeMenu">包括授权菜单</param>
        /// <param name="includeResources">包括授权资源</param>
        /// <param name="includeCFUC">包括授权文件上传配置</param>
        /// <param name="mergeRoleMenu">合并角色的授权菜单</param>
        /// <param name="mergeRoleResources">合并角色的授权资源</param>
        /// <param name="mergeRoleCFUC">合并角色的授权文件上传配置</param>
        /// <returns>
        /// <para>用户授权信息</para>
        /// <para>角色授权信息</para>
        /// <para>菜单授权信息</para>
        /// <para>资源授权信息</para>
        /// </returns>
        Model.System.UserDTO.Authorities GetUser(string userId, bool includeRole, bool includeMenu, bool includeCFUC, bool includeResources, bool mergeRoleMenu = true, bool mergeRoleResources = true, bool mergeRoleCFUC = true);

        /// <summary>
        /// 获取会员的授权数据
        /// </summary>
        /// <param name="memberId">会员Id</param>
        /// <param name="includeRole">包括授权角色</param>
        /// <param name="includeMenu">包括授权菜单</param>
        /// <param name="includeResources">包括授权资源</param>
        /// <param name="includeCFUC">包括授权文件上传配置</param>
        /// <returns>
        /// <para>用户授权信息</para>
        /// <para>角色授权信息</para>
        /// <para>菜单授权信息</para>
        /// <para>资源授权信息</para>
        /// </returns>
        Model.Public.MemberDTO.Authorities GetMember(string memberId, bool includeRole, bool includeMenu, bool includeResources, bool includeCFUC);

        /// <summary>
        /// 获取授权给用户的角色
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="includeMenu">包括授权菜单</param>
        /// <param name="includeResources">包括授权资源</param>
        /// <param name="includeCFUC">包括授权文件上传配置</param>
        /// <returns>
        /// <para>角色授权信息</para>
        /// <para>菜单授权信息</para>
        /// <para>资源授权信息</para>
        /// </returns>
        List<Model.System.RoleDTO.Authorities> GetUserRole(string userId, bool includeMenu, bool includeResources, bool includeCFUC);

        /// <summary>
        /// 获取授权给会员的角色
        /// </summary>
        /// <param name="memberId">会员Id</param>
        /// <param name="includeMenu">包括授权菜单</param>
        /// <param name="includeResources">包括授权资源</param>
        /// <param name="includeCFUC">包括授权文件上传配置</param>
        /// <returns>
        /// <para>角色授权信息</para>
        /// <para>菜单授权信息</para>
        /// <para>资源授权信息</para>
        /// </returns>
        List<Model.System.RoleDTO.Authorities> GetMemberRole(string memberId, bool includeMenu, bool includeResources, bool includeCFUC);

        /// <summary>
        /// 获取授权给用户的角色类型
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns>
        /// <para>角色类型信息<see cref="Model.System.RoleType"/></para>
        /// </returns>
        List<string> GetUserRoleTypes(string userId);

        /// <summary>
        /// 获取授权给用户的角色名称
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns>角色名称</returns>
        List<string> GetUserRoleNames(string userId);

        /// <summary>
        /// 获取授权给会员的角色类型
        /// </summary>
        /// <param name="memberId">会员Id</param>
        /// <returns>
        /// <para>角色类型信息<see cref="Model.System.RoleType"/></para>
        /// </returns>
        List<string> GetMemberRoleTypes(string memberId);

        /// <summary>
        /// 获取授权给会员的角色名称
        /// </summary>
        /// <param name="memberId">会员Id</param>
        /// <returns>角色名称</returns>
        List<string> GetMemberRoleNames(string memberId);

        /// <summary>
        /// 获取授权给用户的菜单
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="mergeRoleMenu">包括角色的授权菜单</param>
        /// <returns>
        /// <para>菜单授权信息</para>
        /// </returns>
        List<Model.System.MenuDTO.Authorities> GetUserMenu(string userId, bool mergeRoleMenu);

        /// <summary>
        /// 获取授权给会员的菜单
        /// </summary>
        /// <param name="memberId">会员Id</param>
        /// <returns>
        /// <para>菜单授权信息</para>
        /// </returns>
        List<Model.System.MenuDTO.Authorities> GetMemberMenu(string memberId);

        /// <summary>
        /// 获权授权给用户的资源
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="mergeRoleResources">包括角色的授权资源</param>
        /// <param name="pagination"></param>
        /// <returns>
        /// <para>资源授权信息</para>
        /// </returns>
        List<Model.System.ResourcesDTO.Authorities> GetUserResources(string userId, bool mergeRoleResources, PaginationDTO pagination = null);

        /// <summary>
        /// 获权授权给会员的资源
        /// </summary>
        /// <param name="memberId">会员Id</param>
        /// <param name="pagination">分页设置</param>
        /// <returns>
        /// <para>资源授权信息</para>
        /// </returns>
        List<Model.System.ResourcesDTO.Authorities> GetMemberResources(string memberId, PaginationDTO pagination = null);

        /// <summary>
        /// 获权授权给用户的文件上传配置
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="mergeRoleCFUC">包括角色的授权文件上传配置</param>
        /// <returns>
        /// <para>文件上传配置授权信息</para>
        /// </returns>
        List<Model.Common.FileUploadConfigDTO.Authorities> GetUserCFUC(string userId, bool mergeRoleCFUC);

        /// <summary>
        /// 获权授权给会员的文件上传配置
        /// </summary>
        /// <param name="memberId">会员Id</param>
        /// <returns>
        /// <para>文件上传配置授权信息</para>
        /// </returns>
        List<Model.Common.FileUploadConfigDTO.Authorities> GetMemberCFUC(string memberId);

        /// <summary>
        /// 获取角色的授权数据
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="includeMenu">包括授权菜单</param>
        /// <param name="includeResources">包括授权资源</param>
        /// <param name="includeCFUC">包括授权文件上传配置</param>
        /// <returns>
        /// <para>角色授权信息</para>
        /// <para>菜单授权信息</para>
        /// <para>资源授权信息</para>
        /// <para>文件上传配置授权信息</para>
        /// </returns>
        Model.System.RoleDTO.Authorities GetRole(string roleId, bool includeMenu, bool includeResources, bool includeCFUC);

        /// <summary>
        /// 获取授权给角色的菜单
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <returns>
        /// <para>菜单授权信息</para>
        /// </returns>
        List<Model.System.MenuDTO.Authorities> GetRoleMenu(string roleId);

        /// <summary>
        /// 获取授权给角色的资源
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="pagination">分页设置</param>
        /// <returns>
        /// <para>资源授权信息</para>
        /// </returns>
        List<Model.System.ResourcesDTO.Authorities> GetRoleResources(string roleId, PaginationDTO pagination);

        /// <summary>
        /// 获取授权给角色的文件上传配置
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <returns>
        /// <para>文件上传配置授权信息</para>
        /// </returns>
        List<Model.Common.FileUploadConfigDTO.Authorities> GetRoleCFUC(string roleId);

        /// <summary>
        /// 获取用户角色授权树状列表数据
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="pagination">分页设置</param>
        /// <returns></returns>
        List<Model.System.RoleDTO.AuthoritiesTree> GetUserRoleTree(string userId, TreePaginationDTO pagination);

        /// <summary>
        /// 获取会员角色授权树状列表数据
        /// </summary>
        /// <param name="memberId">会员ID</param>
        /// <param name="pagination">分页设置</param>
        /// <returns></returns>
        List<Model.System.RoleDTO.AuthoritiesTree> GetMemberRoleTree(string memberId, TreePaginationDTO pagination);

        /// <summary>
        /// 获取当前登录账号的角色授权树状列表数据
        /// </summary>
        /// <param name="pagination">分页设置</param>
        /// <returns></returns>
        List<Model.System.RoleDTO.AuthoritiesTree> GetCurrentAccountRoleTree(TreePaginationDTO pagination);

        /// <summary>
        /// 获取角色菜单授权树状列表数据
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <param name="pagination">分页设置</param>
        /// <returns></returns>
        List<Model.System.MenuDTO.AuthoritiesTree> GetRoleMenuTree(string roleId, TreePaginationDTO pagination);

        /// <summary>
        /// 获取用户菜单授权树状列表数据
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="pagination">分页设置</param>
        /// <returns></returns>
        List<Model.System.MenuDTO.AuthoritiesTree> GetUserMenuTree(string userId, TreePaginationDTO pagination);

        /// <summary>
        /// 获取会员菜单授权树状列表数据
        /// </summary>
        /// <param name="memberId">会员ID</param>
        /// <param name="pagination">分页设置</param>
        /// <returns></returns>
        List<Model.System.MenuDTO.AuthoritiesTree> GetMemberMenuTree(string memberId, TreePaginationDTO pagination);

        /// <summary>
        /// 获取当前登录账号的菜单授权树状列表数据
        /// </summary>
        /// <param name="pagination">分页设置</param>
        /// <returns></returns>
        List<Model.System.MenuDTO.AuthoritiesTree> GetCurrentAccountMenuTree(TreePaginationDTO pagination);

        /// <summary>
        /// 获取角色的授权资源列表数据
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="pagination">分页设置</param>
        /// <returns></returns>
        List<Model.System.ResourcesDTO.Authorities> GetRoleResourcesList(string roleId, PaginationDTO pagination);

        /// <summary>
        /// 获取用户的授权资源列表数据
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="pagination">分页设置</param>
        /// <returns></returns>
        List<Model.System.ResourcesDTO.Authorities> GetUserResourcesList(string userId, PaginationDTO pagination);

        /// <summary>
        /// 获取当前登录账号的资源授权列表数据
        /// </summary>
        /// <param name="pagination">分页设置</param>
        /// <returns></returns>
        List<Model.System.ResourcesDTO.Authorities> GetCurrentAccountResourcesList(PaginationDTO pagination);

        /// <summary>
        /// 获取角色文件上传配置授权树状列表数据
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <param name="pagination">分页设置</param>
        /// <returns></returns>
        List<Model.Common.FileUploadConfigDTO.AuthoritiesTree> GetRoleCFUCTree(string roleId, TreePaginationDTO pagination);

        /// <summary>
        /// 获取用户文件上传配置授权树状列表数据
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="pagination">分页设置</param>
        /// <returns></returns>
        List<Model.Common.FileUploadConfigDTO.AuthoritiesTree> GetUserCFUCTree(string userId, TreePaginationDTO pagination);

        /// <summary>
        /// 获取会员文件上传配置授权树状列表数据
        /// </summary>
        /// <param name="memberId">会员ID</param>
        /// <param name="pagination">分页设置</param>
        /// <returns></returns>
        List<Model.Common.FileUploadConfigDTO.AuthoritiesTree> GetMemberCFUCTree(string memberId, TreePaginationDTO pagination);

        /// <summary>
        /// 获取当前登录账号的文件上传配置授权树状列表数据
        /// </summary>
        /// <param name="pagination">分页设置</param>
        /// <returns></returns>
        List<Model.Common.FileUploadConfigDTO.AuthoritiesTree> GetCurrentAccountCFUCTree(TreePaginationDTO pagination);

        #endregion

        #region 验证授权

        /// <summary>
        /// 是否为超级管理员
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="checkEnable">检查是否已启用</param>
        /// <returns></returns>
        bool IsSuperAdminUser(string userId, bool checkEnable = true);

        /// <summary>
        /// 是否为超级管理角色
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="checkEnable">检查是否已启用</param>
        /// <returns></returns>
        bool IsSuperAdminRole(string roleId, bool checkEnable = true);

        /// <summary>
        /// 是否为管理员
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="checkEnable">检查是否已启用</param>
        /// <returns></returns>
        bool IsAdminUser(string userId, bool checkEnable = true);

        /// <summary>
        /// 是否为管理角色
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="checkEnable">检查是否已启用</param>
        /// <returns></returns>
        bool IsAdminRole(string roleId, bool checkEnable = true);

        /// <summary>
        /// 用户是否拥有角色授权
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="roleId">角色Id</param>
        /// <param name="checkEnable">检查是否已启用</param>
        /// <returns></returns>
        bool UserHasRole(string userId, string roleId, bool checkEnable = true);

        /// <summary>
        /// 用户是否拥有某类型角色的授权
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="roleType">角色类型</param>
        /// <param name="checkEnable">检查是否已启用</param>
        /// <returns></returns>
        bool UserHasRoleType(string userId, string roleType, bool checkEnable = true);

        /// <summary>
        /// 会员是否拥有角色授权
        /// </summary>
        /// <param name="memberId">会员Id</param>
        /// <param name="roleId">角色Id</param>
        /// <param name="checkEnable">检查是否已启用</param>
        /// <returns></returns>
        bool MemberHasRole(string memberId, string roleId, bool checkEnable = true);

        /// <summary>
        /// 会员是否拥有某类型角色的授权
        /// </summary>
        /// <param name="memberId">会员Id</param>
        /// <param name="roleType">角色类型</param>
        /// <param name="checkEnable">检查是否已启用</param>
        /// <returns></returns>
        bool MemberHasRoleType(string memberId, string roleType, bool checkEnable = true);

        /// <summary>
        /// 用户是否拥有菜单授权
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="menuId">菜单Id</param>
        /// <param name="checkEnable">检查是否已启用</param>
        /// <returns></returns>
        bool UserHasMenu(string userId, string menuId, bool checkEnable = true);

        /// <summary>
        /// 用户是否拥有菜单授权
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="menuUri">菜单链接</param>
        /// <param name="checkEnable">检查是否已启用</param>
        /// <returns></returns>
        bool UserHasMenuUri(string userId, string menuUri, bool checkEnable = true);

        /// <summary>
        /// 会员是否拥有菜单授权
        /// </summary>
        /// <param name="memberId">会员Id</param>
        /// <param name="menuId">菜单Id</param>
        /// <param name="checkEnable">检查是否已启用</param>
        /// <returns></returns>
        bool MemberHasMenu(string memberId, string menuId, bool checkEnable = true);

        /// <summary>
        /// 会员是否拥有菜单授权
        /// </summary>
        /// <param name="memberId">会员Id</param>
        /// <param name="menuUri">菜单链接</param>
        /// <param name="checkEnable">检查是否已启用</param>
        /// <returns></returns>
        bool MemberHasMenuUri(string memberId, string menuUri, bool checkEnable = true);

        /// <summary>
        /// 用户是否拥有资源授权
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="resourcesId">资源Id</param>
        /// <param name="checkEnable">检查是否已启用</param>
        /// <returns></returns>
        bool UserHasResources(string userId, string resourcesId, bool checkEnable = true);

        /// <summary>
        /// 用户是否拥有资源授权
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="resourcesUri">资源链接</param>
        /// <param name="checkEnable">检查是否已启用</param>
        /// <returns></returns>
        bool UserHasResourcesUri(string userId, string resourcesUri, bool checkEnable = true);

        /// <summary>
        /// 会员是否拥有资源授权
        /// </summary>
        /// <param name="memberId">会员Id</param>
        /// <param name="resourcesId">资源Id</param>
        /// <param name="checkEnable">检查是否已启用</param>
        /// <returns></returns>
        bool MemberHasResources(string memberId, string resourcesId, bool checkEnable = true);

        /// <summary>
        /// 会员是否拥有资源授权
        /// </summary>
        /// <param name="memberId">会员Id</param>
        /// <param name="resourcesUri">资源链接</param>
        /// <param name="checkEnable">检查是否已启用</param>
        /// <returns></returns>
        bool MemberHasResourcesUri(string memberId, string resourcesUri, bool checkEnable = true);

        /// <summary>
        /// 用户是否拥有文件上传配置授权
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="configId">文件上传配置Id</param>
        /// <param name="checkEnable">检查是否已启用</param>
        /// <returns></returns>
        bool UserHasCFUC(string userId, string configId, bool checkEnable = true);

        /// <summary>
        /// 会员是否拥有文件上传配置授权
        /// </summary>
        /// <param name="memberId">会员Id</param>
        /// <param name="configId">文件上传配置Id</param>
        /// <param name="checkEnable">检查是否已启用</param>
        /// <returns></returns>
        bool MemberHasCFUC(string memberId, string configId, bool checkEnable = true);

        /// <summary>
        /// 当前登录账号是否拥有文件上传配置授权
        /// </summary>
        /// <param name="configId">文件上传配置Id</param>
        /// <param name="checkEnable">检查是否已启用</param>
        /// <returns></returns>
        bool CurrentAccountHasCFUC(string configId, bool checkEnable = true);

        #endregion

        #region 拓展功能



        #endregion
    }
}

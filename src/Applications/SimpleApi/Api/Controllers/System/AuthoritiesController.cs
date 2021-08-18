using Api.Controllers.Utils;
using Business.Interface.System;
using Business.Utils.Authorization;
using Microservice.Library.Extension;
using Microsoft.AspNetCore.Mvc;
using Model.System.AuthorizeDTO;
using Model.Utils.Pagination;
using Model.Utils.Result;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Api.Controllers
{
    /// <summary>
    /// 权限接口
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "权限模块")]
    [Route("/authorities")]
    [SampleAuthorize(nameof(ApiAuthorizeRequirement))]
    public class AuthoritiesController : BaseApiController
    {
        #region DI

        public AuthoritiesController(IAuthoritiesBusiness authoritiesBusiness)
        {
            AuthoritiesBusiness = authoritiesBusiness;
        }

        readonly IAuthoritiesBusiness AuthoritiesBusiness;

        #endregion

        #region 授权接口

        /// <summary>
        /// 为用户授权角色
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        [HttpPost("authorize-role-for-user")]
        [SwaggerOperation(Tags = new[] { "授权" })]
        public async Task<object> AuthorizeRoleForUser([FromBody] RoleForUser data)
        {
            AuthoritiesBusiness.AuthorizeRoleForUser(data);
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 为用户授权角色
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="allChilds">同时授权所有子集角色</param>
        /// <returns></returns>
        [HttpPost("authorize-role-for-user/{roleId}/{userId}")]
        [SwaggerOperation(Tags = new[] { "授权" })]
        public async Task<object> AuthorizeRoleForUser(string roleId, string userId, bool allChilds = false)
        {
            AuthoritiesBusiness.AuthorizeRoleForUser(roleId, userId, allChilds);
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 为会员授权角色
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        [HttpPost("authorize-role-for-member")]
        [SwaggerOperation(Tags = new[] { "授权" })]
        public async Task<object> AuthorizeRoleForMember([FromBody] RoleForMember data)
        {
            AuthoritiesBusiness.AuthorizeRoleForMember(data);
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 为会员授权角色
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="memberId">会员Id</param>
        /// <param name="allChilds">同时授权所有子集角色</param>
        /// <returns></returns>
        [HttpPost("authorize-role-for-member/{roleId}/{memberId}")]
        [SwaggerOperation(Tags = new[] { "授权" })]
        public async Task<object> AuthorizeRoleForMember(string roleId, string memberId, bool allChilds = false)
        {
            AuthoritiesBusiness.AuthorizeRoleForMember(roleId, memberId, allChilds);
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 为用户授权菜单
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        [HttpPost("authorize-menu-for-user")]
        [SwaggerOperation(Tags = new[] { "授权" })]
        public async Task<object> AuthorizeMenuForUser([FromBody] MenuForUser data)
        {
            AuthoritiesBusiness.AuthorizeMenuForUser(data);
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 为用户授权菜单
        /// </summary>
        /// <param name="menuId">菜单Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="allChilds">同时授权所有子集菜单</param>
        /// <returns></returns>
        [HttpPost("authorize-menu-for-user/{menuId}/{userId}")]
        [SwaggerOperation(Tags = new[] { "授权" })]
        public async Task<object> AuthorizeMenuForUser(string menuId, string userId, bool allChilds = false)
        {
            AuthoritiesBusiness.AuthorizeMenuForUser(menuId, userId, allChilds);
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 为用户授权资源
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        [HttpPost("authorize-resources-for-user")]
        [SwaggerOperation(Tags = new[] { "授权" })]
        public async Task<object> AuthorizeResourcesForUser([FromBody] ResourcesForUser data)
        {
            AuthoritiesBusiness.AuthorizeResourcesForUser(data);
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 为角色授权菜单
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        [HttpPost("authorize-menu-for-role")]
        [SwaggerOperation(Tags = new[] { "授权" })]
        public async Task<object> AuthorizeMenuForRole([FromBody] MenuForRole data)
        {
            AuthoritiesBusiness.AuthorizeMenuForRole(data);
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 为角色授权菜单
        /// </summary>
        /// <param name="menuId">菜单Id</param>
        /// <param name="roleId">角色Id</param>
        /// <param name="allChilds">同时授权所有子集菜单</param>
        /// <returns></returns>
        [HttpPost("authorize-menu-for-role/{menuId}/{roleId}")]
        [SwaggerOperation(Tags = new[] { "授权" })]
        public async Task<object> AuthorizeMenuForRole(string menuId, string roleId, bool allChilds = false)
        {
            AuthoritiesBusiness.AuthorizeMenuForRole(menuId, roleId, allChilds);
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 为角色授权资源
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        [HttpPost("authorize-resources-for-role")]
        [SwaggerOperation(Tags = new[] { "授权" })]
        public async Task<object> AuthorizeResourcesForRole([FromBody] ResourcesForRole data)
        {
            AuthoritiesBusiness.AuthorizeResourcesForRole(data);
            return await Task.FromResult(Success());
        }

        #endregion

        #region 撤销授权接口

        /// <summary>
        /// 撤销角色的全部系统用户授权
        /// </summary>
        /// <param name="roleIds">角色Id集合</param>
        /// <returns></returns>
        [HttpPost("revocation-role-for-all-user")]
        [SwaggerOperation(Tags = new[] { "撤销授权" })]
        public async Task<object> RevocationRoleForAllUser(IEnumerable<string> roleIds)
        {
            AuthoritiesBusiness.RevocationRoleForAllUser(roleIds?.ToList());
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 撤销角色的全部会员授权
        /// </summary>
        /// <param name="roleIds">角色Id集合</param>
        /// <returns></returns>
        [HttpPost("revocation-role-for-all-member")]
        [SwaggerOperation(Tags = new[] { "撤销授权" })]
        public async Task<object> RevocationRoleForAllMember(IEnumerable<string> roleIds)
        {
            AuthoritiesBusiness.RevocationRoleForAllMember(roleIds?.ToList());
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 撤销用户的全部角色授权
        /// </summary>
        /// <param name="userIds">用户Id集合</param>
        /// <returns></returns>
        [HttpPost("revocation-all-role-for-user")]
        [SwaggerOperation(Tags = new[] { "撤销授权" })]
        public async Task<object> RevocationRoleForUser(IEnumerable<string> userIds)
        {
            AuthoritiesBusiness.RevocationRoleForUser(userIds?.ToList());
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 撤销会员的全部角色授权
        /// </summary>
        /// <param name="memberIds">会员Id集合</param>
        /// <returns></returns>
        [HttpPost("revocation-all-role-for-member")]
        [SwaggerOperation(Tags = new[] { "撤销授权" })]
        public async Task<object> RevocationRoleForMember(IEnumerable<string> memberIds)
        {
            AuthoritiesBusiness.RevocationRoleForMember(memberIds?.ToList());
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 撤销用户的全部菜单授权
        /// </summary>
        /// <param name="userIds">用户Id集合</param>
        /// <returns></returns>
        [HttpPost("revocation-all-menu-for-user")]
        [SwaggerOperation(Tags = new[] { "撤销授权" })]
        public async Task<object> RevocationMenuForUser(IEnumerable<string> userIds)
        {
            AuthoritiesBusiness.RevocationMenuForUser(userIds?.ToList());
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 撤销用户的全部资源授权
        /// </summary>
        /// <param name="userIds">用户Id集合</param>
        /// <returns></returns>
        [HttpPost("revocation-all-resources-for-user")]
        [SwaggerOperation(Tags = new[] { "撤销授权" })]
        public async Task<object> RevocationResourcesForUser(IEnumerable<string> userIds)
        {
            AuthoritiesBusiness.RevocationResourcesForUser(userIds?.ToList());
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 撤销角色的全部菜单授权
        /// </summary>
        /// <param name="roleIds">角色Id集合</param>
        /// <returns></returns>
        [HttpPost("revocation-all-menu-for-role")]
        [SwaggerOperation(Tags = new[] { "撤销授权" })]
        public async Task<object> RevocationMenuForRole(IEnumerable<string> roleIds)
        {
            AuthoritiesBusiness.RevocationMenuForRole(roleIds?.ToList());
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 撤销角色的全部资源授权
        /// </summary>
        /// <param name="roleIds">角色Id集合</param>
        /// <returns></returns>
        [HttpPost("revocation-all-resources-for-role")]
        [SwaggerOperation(Tags = new[] { "撤销授权" })]
        public async Task<object> RevocationResourcesForRole(IEnumerable<string> roleIds)
        {
            AuthoritiesBusiness.RevocationResourcesForRole(roleIds?.ToList());
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 撤销用户的角色授权
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        [HttpPost("revocation-role-for-user")]
        [SwaggerOperation(Tags = new[] { "撤销授权" })]
        public async Task<object> RevocationRoleForUser([FromBody] RoleForUser data)
        {
            AuthoritiesBusiness.RevocationRoleForUser(data);
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 撤销用户的角色授权
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="allChilds">同时撤销所有子集角色授权</param>
        /// <returns></returns>
        [HttpPost("revocation-role-for-user/{roleId}/{userId}")]
        [SwaggerOperation(Tags = new[] { "撤销授权" })]
        public async Task<object> RevocationRoleForUser(string roleId, string userId, bool allChilds = false)
        {
            AuthoritiesBusiness.RevocationRoleForUser(roleId, userId, allChilds);
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 撤销会员的角色授权
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        [HttpPost("revocation-role-for-member")]
        [SwaggerOperation(Tags = new[] { "撤销授权" })]
        public async Task<object> RevocationRoleForMember([FromBody] RoleForMember data)
        {
            AuthoritiesBusiness.RevocationRoleForMember(data);
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 撤销会员的角色授权
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="memberId">会员Id</param>
        /// <param name="allChilds">同时撤销所有子集角色授权</param>
        /// <returns></returns>
        [HttpPost("revocation-role-for-member/{roleId}/{memberId}")]
        [SwaggerOperation(Tags = new[] { "撤销授权" })]
        public async Task<object> RevocationRoleForMember(string roleId, string memberId, bool allChilds = false)
        {
            AuthoritiesBusiness.RevocationRoleForMember(roleId, memberId, allChilds);
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 撤销用户的菜单授权
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        [HttpPost("revocation-menu-for-user")]
        [SwaggerOperation(Tags = new[] { "撤销授权" })]
        public async Task<object> RevocationMenuForUser([FromBody] MenuForUser data)
        {
            AuthoritiesBusiness.RevocationMenuForUser(data);
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 撤销用户的菜单授权
        /// </summary>
        /// <param name="menuId">菜单Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="allChilds">同时撤销所有子集菜单授权</param>
        /// <returns></returns>
        [HttpPost("revocation-menu-for-user/{menuId}/{userId}")]
        [SwaggerOperation(Tags = new[] { "撤销授权" })]
        public async Task<object> RevocationMenuForUser(string menuId, string userId, bool allChilds = false)
        {
            AuthoritiesBusiness.RevocationMenuForUser(menuId, userId, allChilds);
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 撤销用户的资源授权
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        [HttpPost("revocation-resources-for-user")]
        [SwaggerOperation(Tags = new[] { "撤销授权" })]
        public async Task<object> RevocationResourcesForUser([FromBody] ResourcesForUser data)
        {
            AuthoritiesBusiness.RevocationResourcesForUser(data);
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 撤销角色的菜单授权
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        [HttpPost("revocation-menu-for-role")]
        [SwaggerOperation(Tags = new[] { "撤销授权" })]
        public async Task<object> RevocationMenuForRole([FromBody] MenuForRole data)
        {
            AuthoritiesBusiness.RevocationMenuForRole(data);
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 撤销角色的菜单授权
        /// </summary>
        /// <param name="menuId">菜单Id</param>
        /// <param name="roleId">角色Id</param>
        /// <param name="allChilds">同时撤销所有子集菜单授权</param>
        /// <returns></returns>
        [HttpPost("revocation-menu-for-role/{menuId}/{roleId}")]
        [SwaggerOperation(Tags = new[] { "撤销授权" })]
        public async Task<object> RevocationMenuForRole(string menuId, string roleId, bool allChilds = false)
        {
            AuthoritiesBusiness.RevocationMenuForRole(menuId, roleId, allChilds);
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 撤销角色的资源授权
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        [HttpPost("revocation-resources-for-role")]
        [SwaggerOperation(Tags = new[] { "撤销授权" })]
        public async Task<object> RevocationResourcesForRole([FromBody] ResourcesForRole data)
        {
            AuthoritiesBusiness.RevocationResourcesForRole(data);
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 撤销所有用户和角色的菜单授权
        /// </summary>
        /// <param name="menuIds">菜单Id集合</param>
        /// <returns></returns>
        [HttpPost("revocation-menu-for-all")]
        [SwaggerOperation(Tags = new[] { "撤销授权" })]
        public async Task<object> RevocationMenuForAll(IEnumerable<string> menuIds)
        {
            AuthoritiesBusiness.RevocationMenuForAll(menuIds?.ToList());
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 撤销所有用户和角色的资源授权
        /// </summary>
        /// <param name="resourcesIds">资源Id集合</param>
        /// <returns></returns>
        [HttpPost("revocation-resources-for-all")]
        [SwaggerOperation(Tags = new[] { "撤销授权" })]
        public async Task<object> RevocationResourcesForAll(IEnumerable<string> resourcesIds)
        {
            AuthoritiesBusiness.RevocationResourcesForAll(resourcesIds?.ToList());
            return await Task.FromResult(Success());
        }

        #endregion

        #region 获取授权接口

        #region 暂时不开放

        /// <summary>
        /// 获取用户的授权数据
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="includeRole">包括授权角色</param>
        /// <param name="includeMenu">包括授权菜单</param>
        /// <param name="includeResources">包括授权资源</param>
        /// <param name="mergeRoleMenu">合并角色的授权菜单</param>
        /// <param name="mergeRoleResources">合并角色的授权资源</param>
        /// <returns></returns>
        [NonAction]
        [HttpPost("user-data")]
        [SwaggerOperation(Tags = new[] { "获取授权" })]
        [SwaggerResponse((int)HttpStatusCode.OK, "授权数据", typeof(Model.System.UserDTO.Authorities))]
        public async Task<object> GetUser(string userId, bool includeRole, bool includeMenu, bool includeResources, bool mergeRoleMenu = true, bool mergeRoleResources = true)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(AuthoritiesBusiness.GetUser(userId, includeRole, includeMenu, includeResources, mergeRoleMenu, mergeRoleResources))));
        }

        /// <summary>
        /// 获取会员的授权数据
        /// </summary>
        /// <param name="memberId">会员Id</param>
        /// <param name="includeRole">包括授权角色</param>
        /// <param name="includeMenu">包括授权菜单</param>
        /// <param name="includeResources">包括授权资源</param>
        /// <returns></returns>
        [NonAction]
        [HttpPost("member-data")]
        [SwaggerOperation(Tags = new[] { "获取授权" })]
        [SwaggerResponse((int)HttpStatusCode.OK, "授权数据", typeof(Model.Public.MemberDTO.Authorities))]
        public async Task<object> GetMember(string memberId, bool includeRole, bool includeMenu, bool includeResources)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(AuthoritiesBusiness.GetMember(memberId, includeRole, includeMenu, includeResources))));
        }

        /// <summary>
        /// 获取授权给用户的角色
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="includeMenu">包括授权菜单</param>
        /// <param name="includeResources">包括授权资源</param>
        /// <returns></returns>
        [NonAction]
        [HttpPost("user-role-data")]
        [SwaggerOperation(Tags = new[] { "获取授权" })]
        [SwaggerResponse((int)HttpStatusCode.OK, "授权数据", typeof(Model.System.RoleDTO.Authorities))]
        public async Task<object> GetUserRole(string userId, bool includeMenu, bool includeResources)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(AuthoritiesBusiness.GetUserRole(userId, includeMenu, includeResources))));
        }

        /// <summary>
        /// 获取授权给会员的角色
        /// </summary>
        /// <param name="memberId">会员Id</param>
        /// <param name="includeMenu">包括授权菜单</param>
        /// <param name="includeResources">包括授权资源</param>
        /// <returns></returns>
        [NonAction]
        [HttpPost("member-role-data")]
        [SwaggerOperation(Tags = new[] { "获取授权" })]
        [SwaggerResponse((int)HttpStatusCode.OK, "授权数据", typeof(Model.System.RoleDTO.Authorities))]
        public async Task<object> GetMemberRole(string memberId, bool includeMenu, bool includeResources)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(AuthoritiesBusiness.GetMemberRole(memberId, includeMenu, includeResources))));
        }

        /// <summary>
        /// 获取授权给用户的菜单
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="mergeRoleMenu">包括角色的授权菜单</param>
        /// <returns></returns>
        [NonAction]
        [HttpPost("user-menu-data")]
        [SwaggerOperation(Tags = new[] { "获取授权" })]
        [SwaggerResponse((int)HttpStatusCode.OK, "授权数据", typeof(Model.System.MenuDTO.Authorities))]
        public async Task<object> GetUserMenu(string userId, bool mergeRoleMenu)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(AuthoritiesBusiness.GetUserMenu(userId, mergeRoleMenu))));
        }

        /// <summary>
        /// 获取授权给会员的菜单
        /// </summary>
        /// <param name="memberId">会员Id</param>
        /// <returns></returns>
        [NonAction]
        [HttpPost("member-menu-data")]
        [SwaggerOperation(Tags = new[] { "获取授权" })]
        [SwaggerResponse((int)HttpStatusCode.OK, "授权数据", typeof(Model.System.MenuDTO.Authorities))]
        public async Task<object> GetMemberMenu(string memberId)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(AuthoritiesBusiness.GetMemberMenu(memberId))));
        }

        /// <summary>
        /// 获权授权给用户的资源
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="mergeRoleResources">包括角色的授权资源</param>
        /// <returns></returns>
        [NonAction]
        [HttpPost("user-resources-data")]
        [SwaggerOperation(Tags = new[] { "获取授权" })]
        [SwaggerResponse((int)HttpStatusCode.OK, "授权数据", typeof(Model.System.ResourcesDTO.Authorities))]
        public async Task<object> GetUserResources(string userId, bool mergeRoleResources)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(AuthoritiesBusiness.GetUserResources(userId, mergeRoleResources))));
        }

        /// <summary>
        /// 获权授权给会员的资源
        /// </summary>
        /// <param name="memberId">会员Id</param>
        /// <returns></returns>
        [NonAction]
        [HttpPost("member-resources-data")]
        [SwaggerOperation(Tags = new[] { "获取授权" })]
        [SwaggerResponse((int)HttpStatusCode.OK, "授权数据", typeof(Model.System.ResourcesDTO.Authorities))]
        public async Task<object> GetMemberResources(string memberId)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(AuthoritiesBusiness.GetMemberResources(memberId))));
        }

        /// <summary>
        /// 获取角色的授权数据
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="includeMenu">包括授权菜单</param>
        /// <param name="includeResources">包括授权资源</param>
        /// <returns></returns>
        [NonAction]
        [HttpPost("role-data")]
        [SwaggerOperation(Tags = new[] { "获取授权" })]
        [SwaggerResponse((int)HttpStatusCode.OK, "授权数据", typeof(Model.System.RoleDTO.Authorities))]
        public async Task<object> GetRole(string roleId, bool includeMenu, bool includeResources)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(AuthoritiesBusiness.GetRole(roleId, includeMenu, includeResources))));
        }

        /// <summary>
        /// 获取授权给角色的菜单
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <returns></returns>
        [NonAction]
        [HttpPost("role-menu-data")]
        [SwaggerOperation(Tags = new[] { "获取授权" })]
        [SwaggerResponse((int)HttpStatusCode.OK, "授权数据", typeof(Model.System.MenuDTO.Authorities))]
        public async Task<object> GetRoleMenu(string roleId)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(AuthoritiesBusiness.GetRoleMenu(roleId))));
        }

        /// <summary>
        /// 获取授权给角色的资源
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="pagination">分页设置</param>
        /// <returns></returns>
        [NonAction]
        [HttpPost("role-resources-data")]
        [SwaggerOperation(Tags = new[] { "获取授权" })]
        [SwaggerResponse((int)HttpStatusCode.OK, "授权数据", typeof(Model.System.ResourcesDTO.Authorities))]
        public async Task<object> GetRoleResources(string roleId, [FromBody] PaginationDTO pagination)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(AuthoritiesBusiness.GetRoleResources(roleId, pagination))));
        }

        #endregion

        /// <summary>
        /// 获取用户角色授权树状列表数据
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="paramter">参数</param>
        /// <returns></returns>
        [HttpPost("user-data-role-tree/{userId}")]
        [SwaggerOperation(Tags = new[] { "获取授权" })]
        [SwaggerResponse((int)HttpStatusCode.OK, "授权数据", typeof(Model.System.RoleDTO.AuthoritiesTree))]
        public async Task<object> GetUserRoleTree(string userId, [FromBody] Model.System.RoleDTO.TreeListParamter paramter)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(AuthoritiesBusiness.GetUserRoleTree(userId, paramter))));
        }

        /// <summary>
        /// 获取会员角色授权树状列表数据
        /// </summary>
        /// <param name="memberId">会员ID</param>
        /// <param name="paramter">参数</param>
        /// <returns></returns>
        [HttpPost("member-data-role-tree/{memberId}")]
        [SwaggerOperation(Tags = new[] { "获取授权" })]
        [SwaggerResponse((int)HttpStatusCode.OK, "授权数据", typeof(Model.System.RoleDTO.AuthoritiesTree))]
        public async Task<object> GetMemberRoleTree(string memberId, [FromBody] Model.System.RoleDTO.TreeListParamter paramter)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(AuthoritiesBusiness.GetMemberRoleTree(memberId, paramter))));
        }

        /// <summary>
        /// 获取当前登录账号的角色授权树状列表数据
        /// </summary>
        /// <param name="paramter">参数</param>
        /// <returns></returns>
        [HttpPost("current-account-data-role-tree")]
        [SwaggerOperation(Tags = new[] { "获取授权" })]
        [SwaggerResponse((int)HttpStatusCode.OK, "授权数据", typeof(Model.System.RoleDTO.AuthoritiesTree))]
        public async Task<object> GetCurrentAccountRoleTree([FromBody] Model.System.RoleDTO.TreeListParamter paramter)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(AuthoritiesBusiness.GetCurrentAccountRoleTree(paramter))));
        }

        /// <summary>
        /// 获取角色菜单授权树状列表数据
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <param name="paramter">参数</param>
        /// <returns></returns>
        [HttpPost("role-data-menu-tree/{roleId}")]
        [SwaggerOperation(Tags = new[] { "获取授权" })]
        [SwaggerResponse((int)HttpStatusCode.OK, "授权数据", typeof(Model.System.MenuDTO.AuthoritiesTree))]
        public async Task<object> GetRoleMenuTree(string roleId, [FromBody] Model.System.MenuDTO.TreeListParamter paramter)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(AuthoritiesBusiness.GetRoleMenuTree(roleId, paramter))));
        }

        /// <summary>
        /// 获取用户菜单授权树状列表数据
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="paramter">参数</param>
        /// <returns></returns>
        [HttpPost("user-data-menu-tree/{userId}")]
        [SwaggerOperation(Tags = new[] { "获取授权" })]
        [SwaggerResponse((int)HttpStatusCode.OK, "授权数据", typeof(Model.System.MenuDTO.AuthoritiesTree))]
        public async Task<object> GetUserMenuTree(string userId, [FromBody] Model.System.MenuDTO.TreeListParamter paramter)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(AuthoritiesBusiness.GetUserMenuTree(userId, paramter))));
        }

        /// <summary>
        /// 获取会员菜单授权树状列表数据
        /// </summary>
        /// <param name="memberId">会员ID</param>
        /// <param name="paramter">参数</param>
        /// <returns></returns>
        [HttpPost("member-data-menu-tree/{memberId}")]
        [SwaggerOperation(Tags = new[] { "获取授权" })]
        [SwaggerResponse((int)HttpStatusCode.OK, "授权数据", typeof(Model.System.MenuDTO.AuthoritiesTree))]
        public async Task<object> GetMemberMenuTree(string memberId, [FromBody] Model.System.MenuDTO.TreeListParamter paramter)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(AuthoritiesBusiness.GetMemberMenuTree(memberId, paramter))));
        }

        /// <summary>
        /// 获取当前登录账号的菜单授权树状列表数据
        /// </summary>
        /// <param name="paramter">参数</param>
        /// <returns></returns>
        [HttpPost("current-account-data-menu-tree")]
        [SwaggerOperation(Tags = new[] { "获取授权" })]
        [SwaggerResponse((int)HttpStatusCode.OK, "授权数据", typeof(Model.System.MenuDTO.AuthoritiesTree))]
        public async Task<object> GetCurrentAccountMenuTree([FromBody] Model.System.MenuDTO.TreeListParamter paramter)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(AuthoritiesBusiness.GetCurrentAccountMenuTree(paramter))));
        }

        /// <summary>
        /// 获取角色的授权资源列表数据
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="pagination">分页设置</param>
        /// <returns></returns>
        [HttpPost("role-data-resources-list/{roleId}")]
        [SwaggerOperation(Tags = new[] { "权限相关拓展接口" })]
        [SwaggerResponse((int)HttpStatusCode.OK, "授权数据", typeof(Model.System.ResourcesDTO.Authorities))]
        public async Task<object> GetRoleResourcesList(string roleId, [FromBody] PaginationDTO pagination)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(AuthoritiesBusiness.GetRoleResourcesList(roleId, pagination))));
        }

        /// <summary>
        /// 获取用户的授权资源列表数据
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="pagination">分页设置</param>
        /// <returns></returns>
        [HttpPost("user-data-resources-list/{userId}")]
        [SwaggerOperation(Tags = new[] { "权限相关拓展接口" })]
        [SwaggerResponse((int)HttpStatusCode.OK, "授权数据", typeof(Model.System.ResourcesDTO.Authorities))]
        public async Task<object> GetUserResourcesList(string userId, [FromBody] PaginationDTO pagination)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(AuthoritiesBusiness.GetUserResourcesList(userId, pagination))));
        }

        /// <summary>
        /// 获取当前登录账号的资源授权列表数据
        /// </summary>
        /// <param name="pagination">分页设置</param>
        /// <returns></returns>
        [HttpPost("current-account-data-resources-list")]
        [SwaggerOperation(Tags = new[] { "权限相关拓展接口" })]
        [SwaggerResponse((int)HttpStatusCode.OK, "授权数据", typeof(Model.System.ResourcesDTO.Authorities))]
        public async Task<object> GetCurrentAccountResourcesList([FromBody] PaginationDTO pagination)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(AuthoritiesBusiness.GetCurrentAccountResourcesList(pagination))));
        }

        #endregion

        #region 验证授权接口

        /// <summary>
        /// 是否为超级管理员
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="checkEnable">检查是否已启用（默认true）</param>
        /// <returns></returns>
        [HttpPost("is-super-admin-user")]
        [SwaggerOperation(Tags = new[] { "验证授权" })]
        [SwaggerResponse((int)HttpStatusCode.OK, "验证结果", typeof(bool))]
        public async Task<object> IsSuperAdminUser(string userId, bool checkEnable = true)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(AuthoritiesBusiness.IsSuperAdminUser(userId, checkEnable))));
        }

        /// <summary>
        /// 是否为超级管理角色
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="checkEnable">检查是否已启用（默认true）</param>
        /// <returns></returns>
        [HttpPost("is-super-admin-role")]
        [SwaggerOperation(Tags = new[] { "验证授权" })]
        [SwaggerResponse((int)HttpStatusCode.OK, "验证结果", typeof(bool))]
        public async Task<object> IsSuperAdminRole(string roleId, bool checkEnable = true)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(AuthoritiesBusiness.IsSuperAdminRole(roleId, checkEnable))));
        }

        /// <summary>
        /// 是否为管理员
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="checkEnable">检查是否已启用（默认true）</param>
        /// <returns></returns>
        [HttpPost("is-admin-user")]
        [SwaggerOperation(Tags = new[] { "验证授权" })]
        [SwaggerResponse((int)HttpStatusCode.OK, "验证结果", typeof(bool))]
        public async Task<object> IsAdminUser(string userId, bool checkEnable = true)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(AuthoritiesBusiness.IsAdminUser(userId, checkEnable))));
        }

        /// <summary>
        /// 是否为管理角色
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="checkEnable">检查是否已启用（默认true）</param>
        /// <returns></returns>
        [HttpPost("is-admin-role")]
        [SwaggerOperation(Tags = new[] { "验证授权" })]
        [SwaggerResponse((int)HttpStatusCode.OK, "验证结果", typeof(bool))]
        public async Task<object> IsAdminRole(string roleId, bool checkEnable = true)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(AuthoritiesBusiness.IsAdminRole(roleId, checkEnable))));
        }

        #region 暂时不开放

        /// <summary>
        /// 用户是否拥有角色授权
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="roleId">角色Id</param>
        /// <param name="checkEnable">检查是否已启用（默认true）</param>
        /// <returns></returns>
        [NonAction]
        [HttpPost("user-has-role")]
        [SwaggerOperation(Tags = new[] { "验证授权" })]
        [SwaggerResponse((int)HttpStatusCode.OK, "验证结果", typeof(bool))]
        public async Task<object> UserHasRole(string userId, string roleId, bool checkEnable = true)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(AuthoritiesBusiness.UserHasRole(userId, roleId, checkEnable))));
        }

        /// <summary>
        /// 会员是否拥有角色授权
        /// </summary>
        /// <param name="memberId">会员Id</param>
        /// <param name="roleId">角色Id</param>
        /// <param name="checkEnable">检查是否已启用（默认true）</param>
        /// <returns></returns>
        [NonAction]
        [HttpPost("member-has-role")]
        [SwaggerOperation(Tags = new[] { "验证授权" })]
        [SwaggerResponse((int)HttpStatusCode.OK, "验证结果", typeof(bool))]
        public async Task<object> MemberHasRole(string memberId, string roleId, bool checkEnable = true)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(AuthoritiesBusiness.MemberHasRole(memberId, roleId, checkEnable))));
        }

        /// <summary>
        /// 用户是否拥有菜单授权
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="menuId">菜单Id</param>
        /// <param name="checkEnable">检查是否已启用（默认true）</param>
        /// <returns></returns>
        [NonAction]
        [HttpPost("user-has-menu")]
        [SwaggerOperation(Tags = new[] { "验证授权" })]
        [SwaggerResponse((int)HttpStatusCode.OK, "验证结果", typeof(bool))]
        public async Task<object> UserHasMenu(string userId, string menuId, bool checkEnable = true)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(AuthoritiesBusiness.UserHasMenu(userId, menuId, checkEnable))));
        }

        /// <summary>
        /// 用户是否拥有菜单授权
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="menuUri">菜单链接</param>
        /// <param name="checkEnable">检查是否已启用（默认true）</param>
        /// <returns></returns>
        [NonAction]
        [HttpPost("user-has-menu-uri")]
        [SwaggerOperation(Tags = new[] { "验证授权" })]
        [SwaggerResponse((int)HttpStatusCode.OK, "验证结果", typeof(bool))]
        public async Task<object> UserHasMenuUri(string userId, string menuUri, bool checkEnable = true)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(AuthoritiesBusiness.UserHasMenuUri(userId, menuUri, checkEnable))));
        }

        /// <summary>
        /// 会员是否拥有菜单授权
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="menuId">菜单Id</param>
        /// <param name="checkEnable">检查是否已启用（默认true）</param>
        /// <returns></returns>
        [NonAction]
        [HttpPost("member-has-menu")]
        [SwaggerOperation(Tags = new[] { "验证授权" })]
        [SwaggerResponse((int)HttpStatusCode.OK, "验证结果", typeof(bool))]
        public async Task<object> MemberHasMenu(string userId, string menuId, bool checkEnable = true)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(AuthoritiesBusiness.MemberHasMenu(userId, menuId, checkEnable))));
        }

        /// <summary>
        /// 会员是否拥有菜单授权
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="menuUri">菜单链接</param>
        /// <param name="checkEnable">检查是否已启用（默认true）</param>
        /// <returns></returns>
        [NonAction]
        [HttpPost("member-has-menu-uri")]
        [SwaggerOperation(Tags = new[] { "验证授权" })]
        [SwaggerResponse((int)HttpStatusCode.OK, "验证结果", typeof(bool))]
        public async Task<object> MemberHasMenuUri(string userId, string menuUri, bool checkEnable = true)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(AuthoritiesBusiness.MemberHasMenuUri(userId, menuUri, checkEnable))));
        }

        /// <summary>
        /// 用户是否拥有资源授权
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="resourcesId">资源Id</param>
        /// <param name="checkEnable">检查是否已启用（默认true）</param>
        /// <returns></returns>
        [NonAction]
        [HttpPost("user-has-resources")]
        [SwaggerOperation(Tags = new[] { "验证授权" })]
        [SwaggerResponse((int)HttpStatusCode.OK, "验证结果", typeof(bool))]
        public async Task<object> UserHasResources(string userId, string resourcesId, bool checkEnable = true)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(AuthoritiesBusiness.UserHasResources(userId, resourcesId, checkEnable))));
        }

        /// <summary>
        /// 用户是否拥有资源授权
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="resourcesUri">资源链接</param>
        /// <param name="checkEnable">检查是否已启用（默认true）</param>
        /// <returns></returns>
        [NonAction]
        [HttpPost("user-has-resources-uri")]
        [SwaggerOperation(Tags = new[] { "验证授权" })]
        [SwaggerResponse((int)HttpStatusCode.OK, "验证结果", typeof(bool))]
        public async Task<object> UserHasResourcesUri(string userId, string resourcesUri, bool checkEnable = true)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(AuthoritiesBusiness.UserHasResourcesUri(userId, resourcesUri, checkEnable))));
        }

        /// <summary>
        /// 会员是否拥有资源授权
        /// </summary>
        /// <param name="memberId">会员Id</param>
        /// <param name="resourcesId">资源Id</param>
        /// <param name="checkEnable">检查是否已启用（默认true）</param>
        /// <returns></returns>
        [NonAction]
        [HttpPost("member-has-resources")]
        [SwaggerOperation(Tags = new[] { "验证授权" })]
        [SwaggerResponse((int)HttpStatusCode.OK, "验证结果", typeof(bool))]
        public async Task<object> MemberHasResources(string memberId, string resourcesId, bool checkEnable = true)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(AuthoritiesBusiness.MemberHasResources(memberId, resourcesId, checkEnable))));
        }

        /// <summary>
        /// 会员是否拥有资源授权
        /// </summary>
        /// <param name="memberId">会员Id</param>
        /// <param name="resourcesUri">资源链接</param>
        /// <param name="checkEnable">检查是否已启用（默认true）</param>
        /// <returns></returns>
        [NonAction]
        [HttpPost("member-has-resources-uri")]
        [SwaggerOperation(Tags = new[] { "验证授权" })]
        [SwaggerResponse((int)HttpStatusCode.OK, "验证结果", typeof(bool))]
        public async Task<object> MemberHasResourcesUri(string memberId, string resourcesUri, bool checkEnable = true)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(AuthoritiesBusiness.MemberHasResourcesUri(memberId, resourcesUri, checkEnable))));
        }

        #endregion

        #endregion

        #region 拓展功能



        #endregion
    }
}

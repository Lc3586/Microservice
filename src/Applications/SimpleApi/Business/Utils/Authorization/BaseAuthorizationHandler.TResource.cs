using Business.Interface.System;
using Microservice.Library.Container;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Model.System;
using Model.Utils.Config;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Business.Utils.Authorization
{
    /// <summary>
    /// 自定义授权处理程序基类
    /// </summary>
    /// <typeparam name="TRequirement"></typeparam>
    /// <typeparam name="TResource"></typeparam>
    public class BaseAuthorizationHandler<TRequirement, TResource> :
        AuthorizationHandler<TRequirement, TResource>
        where TRequirement : IAuthorizationRequirement
    {
        SystemConfig _Config;

        /// <summary>
        /// 系统配置
        /// </summary>
        protected SystemConfig Config
        {
            get
            {
                if (_Config == null)
                    _Config = AutofacHelper.GetService<SystemConfig>();
                return _Config;
            }
        }

        IOperator _Operator;

        /// <summary>
        /// 当前操作者
        /// </summary>
        protected IOperator Operator
        {
            get
            {
                if (_Operator == null)
                    _Operator = AutofacHelper.GetScopeService<IOperator>();
                return _Operator;
            }
        }

        IAuthoritiesBusiness _AuthoritiesBusiness;

        /// <summary>
        /// 授权中心
        /// </summary>
        protected IAuthoritiesBusiness AuthoritiesBusiness
        {
            get
            {
                if (_AuthoritiesBusiness == null)
                    _AuthoritiesBusiness = AutofacHelper.GetScopeService<IAuthoritiesBusiness>();
                return _AuthoritiesBusiness;
            }
        }

        /// <summary>
        /// 基础验证
        /// </summary>
        /// <param name="user">用户</param>
        /// <returns></returns>
        protected bool? BaseAuthorization(ClaimsPrincipal user)
        {
            if (Config.RunMode == RunMode.LocalTest)
                return true;

            if (!Operator.IsAuthenticated)
                return false;

            if (Operator.IsSuperAdmin)
                return true;

            return null;
        }

        /// <summary>
        /// 菜单路径验证
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        protected bool MenuUriAuthorization(string uri)
        {
            switch (Operator.AuthenticationInfo.UserType)
            {
                case UserType.系统用户:
                    if (!AuthoritiesBusiness.UserHasMenuUri(Operator.AuthenticationInfo.Id, uri))
                        return false;
                    break;
                case UserType.会员:
                    if (!AuthoritiesBusiness.MemberHasMenuUri(Operator.AuthenticationInfo.Id, uri))
                        return false;
                    break;
                default:
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 资源路径验证
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        protected bool ResourcesUriAuthorization(string uri)
        {
            switch (Operator.AuthenticationInfo.UserType)
            {
                case UserType.系统用户:
                    if (!AuthoritiesBusiness.UserHasResourcesUri(Operator.AuthenticationInfo.Id, uri))
                        return false;
                    break;
                case UserType.会员:
                    if (!AuthoritiesBusiness.MemberHasResourcesUri(Operator.AuthenticationInfo.Id, uri))
                        return false;
                    break;
                default:
                    return false;
            }

            return true;
        }

        protected Task Success(AuthorizationHandlerContext context, TRequirement requirement, ClaimsPrincipal user)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        protected Task Fail(AuthorizationHandlerContext context, TRequirement requirement, ClaimsPrincipal user)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        protected virtual async Task<Task> ResponseError(DefaultHttpContext context, string message, Model.Utils.Result.ErrorCode errorCode)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "text/plain;charset=UTF-8";
            await context.Response.WriteAsync(message);
            return context.Response.CompleteAsync();
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TRequirement requirement, TResource resource)
        {
            return Task.CompletedTask;
        }
    }
}

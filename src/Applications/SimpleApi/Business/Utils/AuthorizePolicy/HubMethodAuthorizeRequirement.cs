using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Model.System;
using Model.Utils.Config;
using Model.Utils.Result;
using System.Threading.Tasks;

namespace Business.Utils.AuthorizePolicy
{
    /// <summary>
    /// SignalR Hub方法权限校验
    /// </summary>
    public class HubMethodAuthorizeRequirement :
        BaseRequirement<HubMethodAuthorizeRequirement, HubInvocationContext>
    {
        public HubMethodAuthorizeRequirement()
        {

        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HubMethodAuthorizeRequirement requirement, HubInvocationContext resource)
        {
            if (Config.RunMode == RunMode.LocalTest)
                goto success;

            if (!Operator.IsAuthenticated)
                return Task.CompletedTask;

            if (Operator.IsSuperAdmin)
                goto success;

            var defaultContext = (DefaultHttpContext)context.Resource;

            //验证权限
            switch (Operator.AuthenticationInfo.UserType)
            {
                case UserType.系统用户:
                    if (!AuthoritiesBusiness.UserHasMenuUri(Operator.AuthenticationInfo.Id, defaultContext.Request.Path.Value?.ToLower()))
                        return Task.CompletedTask;
                    break;
                case UserType.会员:
                    if (!AuthoritiesBusiness.MemberHasMenuUri(Operator.AuthenticationInfo.Id, defaultContext.Request.Path.Value?.ToLower()))
                        return Task.CompletedTask;
                    break;
                default:
                    return Task.CompletedTask;
            }

            success:
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
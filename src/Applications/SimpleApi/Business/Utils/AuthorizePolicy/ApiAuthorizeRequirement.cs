using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Model.System;
using Model.Utils.Config;
using Model.Utils.Result;
using System.Threading.Tasks;

namespace Business.Utils.AuthorizePolicy
{
    /// <summary>
    /// 接口权限校验
    /// </summary>
    public class ApiAuthorizeRequirement :
        BaseRequirement<ApiAuthorizeRequirement>
    {
        public ApiAuthorizeRequirement()
        {

        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ApiAuthorizeRequirement requirement)
        {
            if (Config.RunMode == RunMode.LocalTest)
                goto success;

            if (!Operator.IsAuthenticated)
                goto error;

            if (Operator.IsSuperAdmin)
                goto success;

            var defaultContext = (DefaultHttpContext)context.Resource;

            //验证权限
            switch (Operator.AuthenticationInfo.UserType)
            {
                case UserType.系统用户:
                    if (!AuthoritiesBusiness.UserHasMenuUri(Operator.AuthenticationInfo.Id, defaultContext.Request.Path.Value?.ToLower()))
                        goto error;
                    break;
                case UserType.会员:
                    if (!AuthoritiesBusiness.MemberHasMenuUri(Operator.AuthenticationInfo.Id, defaultContext.Request.Path.Value?.ToLower()))
                        goto error;
                    break;
                default:
                    goto error;
            }

            success:
            context.Succeed(requirement);
            return Task.CompletedTask;

            error:
            context.Fail();
            return Task.CompletedTask;
        }
    }
}
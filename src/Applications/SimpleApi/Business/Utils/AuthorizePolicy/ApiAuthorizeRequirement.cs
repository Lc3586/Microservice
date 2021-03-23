using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Model.System;
using Model.Utils.Config;
using System.Threading.Tasks;

namespace Business.Utils.AuthorizePolicy
{
    /// <summary>
    /// 接口权限校验
    /// </summary>
    public class ApiAuthorizeRequirement :
        BaseRequirement<ApiAuthorizeRequirement>
    {
        /// <summary>
        /// 
        /// </summary>
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

            var type = context.Resource.GetType();

            string uri;
            if (type == typeof(DefaultHttpContext))
                uri = ((DefaultHttpContext)context.Resource).Request.Path.Value?.ToLower();
            else if (type.IsAssignableTo(typeof(AuthorizationFilterContext)))
                uri = ((AuthorizationFilterContext)context.Resource).HttpContext.Request.Path.Value?.ToLower();
            else
                goto success;

            //验证权限
            switch (Operator.AuthenticationInfo.UserType)
            {
                case UserType.系统用户:
                    if (!AuthoritiesBusiness.UserHasResourcesUri(Operator.AuthenticationInfo.Id, uri))
                        goto error;
                    break;
                case UserType.会员:
                    if (!AuthoritiesBusiness.MemberHasResourcesUri(Operator.AuthenticationInfo.Id, uri))
                        goto error;
                    break;
                default:
                    goto error;
            }

            success:
            context.Succeed(requirement);
            return Task.CompletedTask;

            error:
            //context.Fail();
            //return ResponseError(context, "无权限");
            return Task.CompletedTask;

            end:
            return Task.CompletedTask;
        }
    }
}
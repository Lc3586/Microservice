using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace Business.Utils.Authorization
{
    /// <summary>
    /// 接口权限验证处理类
    /// </summary>
    public class ApiPermissionHandlerr<TRequirement> :
         BaseAuthorizationHandler<TRequirement>
         where TRequirement : IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TRequirement requirement)
        {
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// 接口权限验证处理类
    /// </summary>
    public class ApiPermissionDefaultHttpContextHandlerr<TRequirement> :
         BaseAuthorizationHandler<TRequirement, DefaultHttpContext>
         where TRequirement : IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TRequirement requirement, DefaultHttpContext resource)
        {
            var flag = BaseAuthorization(resource.User);
            if (flag == true)
                goto success;
            else if (flag == false)
                goto fail;

            string uri = resource.Request.Path.Value?.ToLower();

            if (!MenuUriAuthorization(uri) && !ResourcesUriAuthorization(uri))
                goto fail;

            success:
            return Success(context, requirement, resource.User);

            fail:
            return Fail(context, requirement, resource.User);
        }
    }

    /// <summary>
    /// 接口权限验证处理类
    /// </summary>
    public class ApiPermissionAuthorizationFilterContextHandlerr<TRequirement> :
         BaseAuthorizationHandler<TRequirement, AuthorizationFilterContext>
         where TRequirement : IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TRequirement requirement, AuthorizationFilterContext resource)
        {
            var flag = BaseAuthorization(resource.HttpContext.User);
            if (flag == true)
                goto success;
            else if (flag == false)
                goto fail;

            string uri = resource.HttpContext.Request.Path.Value?.ToLower();

            if (!MenuUriAuthorization(uri) && !ResourcesUriAuthorization(uri))
                goto fail;

            success:
            return Success(context, requirement, resource.HttpContext.User);

            fail:
            return Fail(context, requirement, resource.HttpContext.User);
        }
    }
}

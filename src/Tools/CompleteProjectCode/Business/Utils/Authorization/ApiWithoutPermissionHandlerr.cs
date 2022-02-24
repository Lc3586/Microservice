using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace Business.Utils.Authorization
{
    /// <summary>
    /// 接口无权限验证处理类
    /// </summary>
    public class ApiWithoutPermissionHandlerr<TRequirement> :
         BaseAuthorizationHandler<TRequirement>
         where TRequirement : IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TRequirement requirement)
        {
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// 接口无权限验证处理类
    /// </summary>
    public class ApiWithoutPermissionDefaultHttpContextHandlerr<TRequirement> :
         BaseAuthorizationHandler<TRequirement, DefaultHttpContext>
         where TRequirement : IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TRequirement requirement, DefaultHttpContext resource)
        {
            var flag = BaseAuthorization(resource.User);
            if (flag == false)
                goto fail;
            else
                goto success;

            success:
            return Success(context, requirement, resource.User);

        fail:
            return Fail(context, requirement, resource.User);
        }
    }

    /// <summary>
    /// 接口无权限验证处理类
    /// </summary>
    public class ApiWithoutPermissionAuthorizationFilterContextHandlerr<TRequirement> :
         BaseAuthorizationHandler<TRequirement, AuthorizationFilterContext>
         where TRequirement : IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TRequirement requirement, AuthorizationFilterContext resource)
        {
            var flag = BaseAuthorization(resource.HttpContext.User);
            if (flag == false)
                goto fail;
            else
                goto success;

            success:
            return Success(context, requirement, resource.HttpContext.User);

        fail:
            return Fail(context, requirement, resource.HttpContext.User);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Model.Utils.Config;
using System.Threading.Tasks;

namespace Business.Utils.AuthorizePolicy
{
    /// <summary>
    /// 允许登录的用户访问，而不判断有没有授权
    /// </summary>
    public class AllowAuthenticatedRequirement :
        BaseRequirement<AllowAuthenticatedRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AllowAuthenticatedRequirement requirement)
        {
            if (Config.RunMode == RunMode.LocalTest)
                goto success;

            if (!Operator.IsAuthenticated)
                return Task.CompletedTask;

            success:
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}

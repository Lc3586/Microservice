using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Model.System;
using Model.Utils.Config;
using System.Threading.Tasks;

namespace Business.Utils.Authorization
{
    /// <summary>
    /// SignalR 集线器权限处理类
    /// </summary>
    public class SignalRHubHandler<TRequirement> :
         BaseAuthorizationHandler<TRequirement, HubInvocationContext>
         where TRequirement : IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TRequirement requirement, HubInvocationContext resource)
        {
            if (Config.RunMode == RunMode.LocalTest)
                goto success;

            if (!Operator.IsAuthenticated)
                return Task.CompletedTask;

            if (Operator.IsSuperAdmin)
                goto success;

            var uri = $"/hub/{resource.HubMethodName.ToLower()}";

            if (!ResourcesUriAuthorization(uri))
                return Task.CompletedTask;

            success:
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Business.Utils.Authorization
{
    /// <summary>
    /// 简易权限验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SampleAuthorizeAttribute :
        Attribute, IAsyncAuthorizationFilter
    {
        /// <summary>
        /// 简易权限验证
        /// </summary>
        /// <param name="policys">授权策略</param>
        public SampleAuthorizeAttribute(params string[] policys)
        {
            Policys = policys;
        }

        /// <summary>
        /// 授权策略
        /// </summary>
        public IEnumerable<string> Policys { get; set; }

        /// <summary>
        /// 必须满足全部授权策略
        /// </summary>
        /// <remarks>默认true</remarks>
        public bool MustAll { get; set; } = true;

        /// <summary>
        /// 验证权限
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (context.ActionDescriptor.EndpointMetadata.Any(o => o.GetType() == typeof(AllowAnonymousAttribute)))
                return;

            ClaimsPrincipal claimsPrincipal = context.HttpContext.User;

            if (!claimsPrincipal.Identity.IsAuthenticated)
            {
                await context.HttpContext.ChallengeAsync();
                return;
            }

            IAuthorizationService authorizationService = (IAuthorizationService)context.HttpContext.RequestServices.GetService(typeof(IAuthorizationService));
            foreach (var policy in Policys)
            {
                AuthorizationResult authorizationResult = await authorizationService.AuthorizeAsync(context.HttpContext.User, context, policy);
                if (authorizationResult.Succeeded)
                {
                    if (!MustAll || Policys.Last() == policy)
                        //成功
                        return;
                }
                else
                {
                    if (MustAll || Policys.Last() == policy)
                    {
                        //失败
                        await context.HttpContext.ForbidAsync();
                        return;
                    }
                }
            }
        }
    }
}

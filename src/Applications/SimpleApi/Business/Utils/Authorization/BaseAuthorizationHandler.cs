using Business.Interface.System;
using Microservice.Library.Container;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Model.Utils.Config;
using System.Threading.Tasks;

namespace Business.Utils.Authorization
{
    /// <summary>
    /// 自定义授权处理程序基类
    /// </summary>
    /// <typeparam name="TRequirement"></typeparam>
    public class BaseAuthorizationHandler<TRequirement> :
        AuthorizationHandler<TRequirement>
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

        /// <summary>
        /// 当前操作者
        /// </summary>
        protected IOperator Operator => AutofacHelper.GetScopeService<IOperator>();

        /// <summary>
        /// 授权中心
        /// </summary>
        protected IAuthoritiesBusiness AuthoritiesBusiness => AutofacHelper.GetScopeService<IAuthoritiesBusiness>();

        protected virtual Task ResponseError(AuthorizationHandlerContext context, string message)
        {
            return ResponseError((DefaultHttpContext)context.Resource, message);
        }

        protected virtual async Task<Task> ResponseError(DefaultHttpContext context, string message)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "text/plain;charset=UTF-8";
            await context.Response.WriteAsync(message);
            return context.Response.CompleteAsync();
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TRequirement requirement)
        {
            return Task.CompletedTask;
        }
    }
}

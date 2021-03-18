using Business.Interface.System;
using Microservice.Library.Container;
using Microservice.Library.Extension;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Model.Utils.Config;
using Model.Utils.Result;
using System.Threading.Tasks;

namespace Business.Utils.AuthorizePolicy
{
    /// <summary>
    /// 自定义授权处理程序基类
    /// </summary>
    /// <typeparam name="TRequirement"></typeparam>
    /// <typeparam name="TResource"></typeparam>
    public class BaseRequirement<TRequirement, TResource> :
        AuthorizationHandler<TRequirement, TResource>,
        IAuthorizationRequirement
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

        protected virtual Task ResponseError(AuthorizationHandlerContext context, string message, Model.Utils.Result.ErrorCode errorCode)
        {
            return ResponseError((DefaultHttpContext)context.Resource, message, errorCode);
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

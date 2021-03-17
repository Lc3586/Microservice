using Business.Interface.System;
using Microservice.Library.Container;
using Microservice.Library.Extension;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Model.Utils.Config;
using Model.Utils.Result;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Utils.AuthorizePolicy
{
    /// <summary>
    /// 自定义授权处理程序基类
    /// </summary>
    /// <typeparam name="TRequirement"></typeparam>
    public class BaseRequirement<TRequirement> :
        AuthorizationHandler<TRequirement>,
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

        /// <summary>
        /// 当前操作者
        /// </summary>
        protected IOperator Operator => AutofacHelper.GetScopeService<IOperator>();

        /// <summary>
        /// 授权中心
        /// </summary>
        protected IAuthoritiesBusiness AuthoritiesBusiness => AutofacHelper.GetScopeService<IAuthoritiesBusiness>();

        protected virtual Task ResponseError(AuthorizationHandlerContext context, string message, Model.Utils.Result.ErrorCode errorCode)
        {
            return ResponseError((DefaultHttpContext)context.Resource, message, errorCode);
        }

        protected virtual Task ResponseError(DefaultHttpContext context, string message, Model.Utils.Result.ErrorCode errorCode)
        {
            var res = new AjaxResult
            {
                Success = false,
                Msg = message,
                ErrorCode = (int)errorCode
            };

            return context.Response.WriteAsync(res.ToJson());
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TRequirement requirement)
        {
            return Task.CompletedTask;
        }
    }
}

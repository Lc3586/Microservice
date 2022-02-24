using Business.Utils;
using Microservice.Library.Extension;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api
{
    /// <summary>
    /// 全局错误拦截
    /// </summary>
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var result = ExceptionHelper.HandleException(context.Exception, context.HttpContext.Request.Path.Value).ToJson();
            if (context.HttpContext.Response.HasStarted)
            {
                context.HttpContext.Response.WriteAsync(result).GetAwaiter().GetResult();
                return;
            }

            context.Result = new ContentResult
            {
                Content = result,
                ContentType = "application/json; charset=utf-8",
            };
        }
    }
}

using Business.Utils;
using Microservice.Library.Extension;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Api
{
    /// <summary>
    /// 异常处理中间件
    /// </summary>
    public class ExceptionHandlerMiddleware
    {
        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            Next = next;
        }

        #region 私有成员

        readonly RequestDelegate Next;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await Next.Invoke(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (!context.Response.HasStarted)
                    context.Response.ContentType = "application/json; charset=utf-8";
                await context.Response.WriteAsync(ExceptionHelper.HandleException(ex, context.Request.Path.Value).ToJson());
            }
        }
    }
}

using Business.Interface.System;
using Microservice.Library.Extension;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.Utils.SampleAuthentication.SampleAuthenticationDTO;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Api.Controllers.Utils
{
    /// <summary>
    /// 简易身份认证接口
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "身份验证模块")]
    [Route("/sa")]
    [SwaggerTag("简易身份认证接口")]
    public class SampleAuthenticationController : BaseApiController
    {
        #region DI

        public SampleAuthenticationController(
            IHttpContextAccessor accessor,
            IUserBusiness userBusiness)
        {
            Context = accessor.HttpContext;
            UserBusiness = userBusiness;
        }

        readonly HttpContext Context;

        readonly IUserBusiness UserBusiness;

        #endregion

        /// <summary>
        /// 登录验证
        /// </summary>
        /// <remarks>
        /// <para>使用此接口判断登录状态</para>
        /// <para>已登录返回身份信息</para>
        /// <para>未登录返回状态码401</para>
        /// </remarks>
        /// <returns>身份信息</returns>
        [HttpPost("authorized")]
        [SwaggerResponse((int)HttpStatusCode.OK, "验证信息", typeof(AuthenticationInfo))]
        public object Authorized()
        {
            return Success(UserBusiness.AnalysisClaims(Context.User.Claims?.ToList()));
        }

        /// <summary>
        /// 无权限/拒绝访问
        /// </summary>
        /// <returns></returns>
        [HttpGet("access-denied")]
        [AllowAnonymous]
        public async Task AccessDenied()
        {
            Context.Response.StatusCode = StatusCodes.Status403Forbidden;
            Context.Response.ContentType = "application/json; charset=utf-8";

            if (Context.User.Identity.IsAuthenticated)
                await Context.Response.WriteAsync("无权限");
            else
                await Context.Response.WriteAsync("拒绝访问");

            //await Task.Run(() => Context.Response.Redirect("/sa/login"));
        }

        /// <summary>
        /// 登录异常
        /// </summary>
        /// <returns></returns>
        [HttpGet("ExternalLoginFailure")]
        [AllowAnonymous]
        public async Task ExternalLoginFailure()
        {
            if (Context.User.Identity.IsAuthenticated)
                await Task.Run(() => Context.Response.Redirect("/sa/login"));
            else
                await Context.Response.WriteAsync("系统繁忙");
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="data">参数</param>
        /// <returns></returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<object> Login([FromBody] LoginRequest data)
        {
            var authenticationInfo = UserBusiness.Login(data.Account, data.Password);

            var claims = UserBusiness.CreateClaims(authenticationInfo, "SA");

            await Context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));

            return Success("登录成功");
        }

        /// <summary>
        /// 注销
        /// </summary>
        /// <param name="data">参数</param>
        /// <returns></returns>
        [HttpGet("logout")]
        public async Task Logout(LogoutRequest data)
        {
            if (data?.ReturnUrl?.ToLower().IndexOf("/sa/logout") >= 0)
                data.ReturnUrl = null;

            var props = new AuthenticationProperties { RedirectUri = data.ReturnUrl };

            await Context.SignOutAsync(props);
        }
    }
}

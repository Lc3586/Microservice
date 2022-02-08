using Business.Interface.System;
using Business.Utils.JWT;
using Microservice.Library.Extension;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.Utils.JWT.JWTDTO;
using Model.Utils.SampleAuthentication.SampleAuthenticationDTO;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq;
using System.Net;
using System.Security.Claims;

namespace Api.Controllers.Utils
{
    /// <summary>
    /// 身份令牌接口
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "身份验证模块")]
    [Route("/jwt")]
    [SwaggerTag("身份令牌接口")]
    public class JWTController : BaseApiController
    {
        #region DI

        public JWTController(
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
        /// 获取令牌
        /// </summary>
        /// <remarks>
        /// <para>返回的Token，放在请求的Header中，"Authorization":"Bearer xxxxxxxxxxxxxx"</para>
        /// </remarks>
        /// <param name="data">参数（已登录的情况下此参数为空）</param>
        /// <returns></returns>
        [HttpPost("get-token")]
        [AllowAnonymous]
        [SwaggerResponse((int)HttpStatusCode.OK, "令牌信息", typeof(TokenInfo))]
        public object GetToken([FromBody] LoginRequest data = null)
        {
            AuthenticationInfo authenticationInfo;
            if (Operator.IsAuthenticated)
                authenticationInfo = Operator.AuthenticationInfo;
            else
                authenticationInfo = UserBusiness.LoginWithLimitTimes(data.Account, data.Password);

            var claims = UserBusiness.CreateClaims(authenticationInfo, "JWT");

            claims.AddRange(authenticationInfo.RoleTypes.Select(o => new Claim(ClaimTypes.Role, o)));

            return Success(JWTHelper.GenerateToken(claims));
        }

        /// <summary>
        /// 刷新令牌
        /// </summary>
        /// <returns></returns>
        [HttpPost("refresh-token")]
        [SwaggerResponse((int)HttpStatusCode.OK, "新的令牌信息", typeof(TokenInfo))]
        public object RefreshToken()
        {
            return Success(JWTHelper.GenerateToken(Context.User.Claims?.ToList()));
        }
    }
}

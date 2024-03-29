﻿using Api.Controllers.Utils;
using Business.Interface.Common;
using Business.Interface.System;
using Business.Utils.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.Common.WeChatUserInfoDTO;
using Model.Utils.Result;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Threading.Tasks;

namespace Api.Controllers
{
    /// <summary>
    /// 微信认证接口
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "微信拓展")]
    [Route("/wechat-oath")]
    [SampleAuthorize(nameof(ApiWithoutPermissionRequirement))]
    [SwaggerTag("微信认证接口")]
    public class WeChatOAuthController : BaseApiController
    {
        #region DI

        public WeChatOAuthController(
            IHttpContextAccessor httpContextAccessor,
            IWeChatUserInfoBusiness weChatUserInfoBusiness,
            IMemberBusiness memberBusiness)
        {
            HttpContextAccessor = httpContextAccessor;
            WeChatUserInfoBusiness = weChatUserInfoBusiness;
            MemberBusiness = memberBusiness;
        }

        readonly IHttpContextAccessor HttpContextAccessor;

        readonly IWeChatUserInfoBusiness WeChatUserInfoBusiness;

        readonly IMemberBusiness MemberBusiness;

        #endregion

        #region 基础接口

        /// <summary>
        /// 会员登录
        /// </summary>
        /// <param name="returnUrl">重定向地址</param>
        [HttpGet("member-login")]
        [AllowAnonymous]
        public void MemberLogin(string returnUrl)
        {
            var state = WeChatUserInfoBusiness.GetState(new StateInfo
            {
                Type = Model.Common.WeChatStateType.会员登录,
                RedirectUrl = returnUrl,
                Data = new System.Collections.Generic.Dictionary<string, object>
                {
                    {
                        "AutoCreate",
                        true
                    }
                }
            });

            var url = $"{Config.WeChatService.OAuthBaseUrl}?state={state}";

            HttpContextAccessor.HttpContext.Response.Redirect(url);
        }

        /// <summary>
        /// 更新会员微信用户信息
        /// </summary>
        /// <param name="memberId">会员Id</param>
        /// <param name="returnUrl">重定向地址</param>
        [HttpGet("member-update/{memberId}")]
        public void UpdateMemberWeChatUserInfo(string memberId, string returnUrl)
        {
            var state = WeChatUserInfoBusiness.GetState(new StateInfo
            {
                Type = Model.Common.WeChatStateType.微信信息同步至会员信息,
                RedirectUrl = returnUrl,
                Data = new System.Collections.Generic.Dictionary<string, object>
                {
                    {
                        "MemberId",
                        memberId
                    }
                }
            });

            var url = $"{Config.WeChatService.OAuthUserInfoUrl}?state={state}";

            HttpContextAccessor.HttpContext.Response.Redirect(url);
        }

        /// <summary>
        /// 获取用于系统用户绑定微信的链接
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="returnUrl">重定向地址</param>
        /// <param name="asyncUserInfo">同步微信信息至用户信息, 默认不同步</param>
        /// <returns></returns>
        [HttpGet("user-bind-url/{userId}")]
        [SwaggerResponse((int)HttpStatusCode.OK, "链接信息", typeof(UrlInfo))]
        public async Task<object> GetUserBindUrl(string userId, string returnUrl, bool asyncUserInfo = false)
        {
            var state = WeChatUserInfoBusiness.GetState(new StateInfo
            {
                Type = Model.Common.WeChatStateType.系统用户绑定微信,
                RedirectUrl = returnUrl,
                Data = new System.Collections.Generic.Dictionary<string, object>
                {
                    {
                        "UserId",
                        userId
                    },
                    {
                        "AsyncUserInfo",
                        asyncUserInfo
                    }
                }
            });

            var url = $"{Config.WebRootUrlMatchScheme(HttpContextAccessor.HttpContext.Request.Scheme)}{Config.WeChatService.OAuthUserInfoUrl}?state={state}";

            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(new UrlInfo
            {
                Url = url,
                S = state
            })));
        }

        /// <summary>
        /// 获取用于系统用户登录的链接
        /// </summary>
        /// <param name="returnUrl">重定向地址</param>
        [HttpGet("user-login-url")]
        [AllowAnonymous]
        [SwaggerResponse((int)HttpStatusCode.OK, "链接信息", typeof(UrlInfo))]
        public async Task<object> GetUserLoginUrl(string returnUrl)
        {
            var state = WeChatUserInfoBusiness.GetState(new StateInfo
            {
                Type = Model.Common.WeChatStateType.系统用户登录,
                RedirectUrl = returnUrl,
                Data = new System.Collections.Generic.Dictionary<string, object>
                {

                }
            });

            var url = $"{Config.WebRootUrlMatchScheme(HttpContextAccessor.HttpContext.Request.Scheme)}{Config.WeChatService.OAuthBaseUrl}?state={state}";

            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(new UrlInfo
            {
                Url = url,
                S = state
            })));
        }

        /// <summary>
        /// 系统用户登录
        /// </summary>
        /// <param name="state"></param>
        /// <param name="token"></param>
        [HttpGet("user-login/{state}/{token}")]
        [AllowAnonymous]
        public async Task<object> UserLogin(string state, string token)
        {
            await WeChatUserInfoBusiness.UserLogin(state, token);
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 获取用于更新系统用户微信用户信息的链接
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="returnUrl">重定向地址</param>
        [HttpGet("user-update-url/{userId}")]
        [SwaggerResponse((int)HttpStatusCode.OK, "链接信息", typeof(UrlInfo))]
        public async Task<object> GetUpdateUserWeChatUserInfoUrl(string userId, string returnUrl)
        {
            var state = WeChatUserInfoBusiness.GetState(new StateInfo
            {
                Type = Model.Common.WeChatStateType.微信信息同步至系统用户信息,
                RedirectUrl = returnUrl,
                Data = new System.Collections.Generic.Dictionary<string, object>
                {
                    {
                        "UserId",
                        userId
                    }
                }
            });

            var url = $"{Config.WebRootUrlMatchScheme(HttpContextAccessor.HttpContext.Request.Scheme)}{Config.WeChatService.OAuthUserInfoUrl}?state={state}";

            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(new UrlInfo
            {
                Url = url,
                S = state
            })));
        }

        /// <summary>
        /// 获取操作说明
        /// </summary>
        /// <param name="state"></param>
        [HttpGet("explain/{state}")]
        [AllowAnonymous]
        public async Task<object> GetExplain(string state)
        {
            return await Task.FromResult(ResponseDataFactory.Success<string>(WeChatUserInfoBusiness.GetExplain(state)));
        }

        /// <summary>
        /// 微信确认操作
        /// </summary>
        /// <param name="state"></param>
        [HttpGet("confirm/{state}")]
        [AllowAnonymous]
        public async Task<object> Confirm(string state)
        {
            await WeChatUserInfoBusiness.Confirm(state);
            return await Task.FromResult(ResponseDataFactory.Success());
        }

        /// <summary>
        /// 微信取消操作
        /// </summary>
        /// <param name="state"></param>
        [HttpGet("cancel/{state}")]
        [AllowAnonymous]
        public async Task<object> Cancel(string state)
        {
            await WeChatUserInfoBusiness.Cancel(state);
            return await Task.FromResult(ResponseDataFactory.Success());
        }

        #endregion

        #region 拓展接口



        #endregion
    }
}

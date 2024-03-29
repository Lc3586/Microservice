﻿using Business.Interface.System;
using Microservice.Library.Container;
using Microsoft.AspNetCore.Http;
using Model.Common;
using Model.Utils.SampleAuthentication.SampleAuthenticationDTO;
using System.Linq;
using System.Security.Claims;

namespace Business.Implementation.System
{
    /// <summary>
    /// 操作者
    /// </summary>
    public class Operator : IOperator, IDependency
    {
        #region DI

        public Operator(
            IHttpContextAccessor httpContextAccessor,
            IAuthoritiesBusiness authoritiesBusiness,
            IUserBusiness userBusiness,
            IMemberBusiness memberBusiness)
        {
            AuthoritiesBusiness = authoritiesBusiness;
            UserBusiness = userBusiness;
            MemberBusiness = memberBusiness;

            IsAuthenticated = httpContextAccessor.HttpContext?.User.Identity.IsAuthenticated == true;

            if (IsAuthenticated)
                AuthenticationInfo = new AuthenticationInfo()
                {
                    Id = httpContextAccessor.HttpContext.User.Claims?.FirstOrDefault(o => o.Type == nameof(AuthenticationInfo.Id))?.Value,

                    Account = httpContextAccessor.HttpContext.User.Claims?.FirstOrDefault(o => o.Type == ClaimTypes.Name)?.Value,

                    UserType = httpContextAccessor.HttpContext.User.Claims?.FirstOrDefault(o => o.Type == nameof(AuthenticationInfo.UserType))?.Value,

                    RoleTypes = httpContextAccessor.HttpContext.User.Claims?.Where(o => o.Type == ClaimTypes.Role).Select(o => o.Value).ToList(),
                    RoleNames = httpContextAccessor.HttpContext.User.Claims?.Where(o => o.Type == nameof(AuthenticationInfo.RoleNames)).Select(o => o.Value).ToList(),

                    Nickname = httpContextAccessor.HttpContext.User.Claims?.FirstOrDefault(o => o.Type == ClaimTypes.GivenName)?.Value,
                    Sex = httpContextAccessor.HttpContext.User.Claims?.FirstOrDefault(o => o.Type == ClaimTypes.Gender)?.Value,

                    Face = httpContextAccessor.HttpContext.User.Claims?.FirstOrDefault(o => o.Type == nameof(AuthenticationInfo.Face))?.Value,

                    AuthenticationMethod = httpContextAccessor.HttpContext.User.Claims?.FirstOrDefault(o => o.Type == ClaimTypes.AuthenticationMethod)?.Value
                };
        }

        readonly IAuthoritiesBusiness AuthoritiesBusiness;

        readonly IUserBusiness UserBusiness;

        readonly IMemberBusiness MemberBusiness;

        #endregion

        #region 外部接口

        /// <summary>
        /// 是否已登录
        /// </summary>
        public bool IsAuthenticated { get; }

        /// <summary>
        /// 当前操作者身份验证信息
        /// </summary>
        public AuthenticationInfo AuthenticationInfo { get; }

        /// <summary>
        /// 用户信息
        /// </summary>
        public OperatorUserInfo UserInfo
        {
            get
            {
                return AuthenticationInfo?.UserType switch
                {
                    Model.System.UserType.系统用户 => UserBusiness.GetOperatorDetail(AuthenticationInfo.Id),
                    Model.System.UserType.会员 => MemberBusiness.GetOperatorDetail(AuthenticationInfo.Id),
                    _ => null
                };
            }
        }

        /// <summary>
        /// 判断是否为超级管理员
        /// </summary>
        /// <returns></returns>
        public bool IsSuperAdmin => AuthenticationInfo.UserType == Model.System.UserType.系统用户 && AuthoritiesBusiness.IsSuperAdminUser(AuthenticationInfo.Id);

        /// <summary>
        /// 判断是否为管理员
        /// </summary>
        /// <returns></returns>
        public bool IsAdmin => AuthenticationInfo.UserType == Model.System.UserType.系统用户 && AuthoritiesBusiness.IsAdminUser(AuthenticationInfo.Id);

        #endregion
    }
}

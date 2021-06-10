using Microservice.Library.Container;
using Microservice.Library.Extension;
using Microsoft.IdentityModel.Tokens;
using Model.Utils.Config;
using Model.Utils.JWT.JWTDTO;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Business.Utils.JWT
{
    /// <summary>
    /// JWT帮助类
    /// </summary>
    public static class JWTHelper
    {
        static SystemConfig Config
        {
            get
            {
                if (_Config == null)
                    _Config = AutofacHelper.GetService<SystemConfig>();
                return _Config;
            }
        }

        static SystemConfig _Config;

        static SymmetricSecurityKey SecurityKey
        {
            get
            {
                if (_SecurityKey == null)
                    _SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Config.JWT.SecurityKey));
                return _SecurityKey;
            }
        }

        static SymmetricSecurityKey _SecurityKey;

        static SigningCredentials Credentials
        {
            get
            {
                if (_Credentials == null)
                    _Credentials = new SigningCredentials(SecurityKey, Config.JWT.Algorithm ?? SecurityAlgorithms.HmacSha256);
                return _Credentials;
            }
        }

        static SigningCredentials _Credentials;

        static JwtSecurityTokenHandler Handler
        {
            get
            {
                if (_Handler == null)
                    _Handler = new JwtSecurityTokenHandler();
                return _Handler;
            }
        }

        static JwtSecurityTokenHandler _Handler;

        /// <summary>
        /// 生成令牌
        /// </summary>
        /// <param name="claims">声明信息</param>
        /// <returns>令牌信息</returns>
        public static TokenInfo GenerateToken(List<Claim> claims)
        {
            claims.RemoveAll(o => o.Type == "iss" || o.Type == "aud");

            var expires = DateTime.Now.AddTicks(Config.JWT.Validity.Ticks);

            var token = new JwtSecurityToken(
                                Config.JWT.Issuer,
                                Config.JWT.Audience,
                                claims,
                                DateTime.Now.AddMinutes(1),
                                expires,
                                Credentials
                        );

            var result = new TokenInfo
            {
                AccessToken = Handler.WriteToken(token),
                Expires = expires
            };

            return result;
        }

        /// <summary>
        /// 分析令牌
        /// </summary>
        /// <param name="accessToken">令牌</param>
        /// <returns>声明信息</returns>
        public static List<Claim> AnalysisToken(string accessToken)
        {
            var principal = Handler.ValidateToken(
                accessToken,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = SecurityKey,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = Config.JWT.Issuer,
                    ValidAudience = Config.JWT.Audience,
                    ValidAlgorithms = new List<string> { Config.JWT.Algorithm }
                },
                out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(Config.JWT.Algorithm, StringComparison.InvariantCultureIgnoreCase))
                throw new MessageException("无效的令牌.");

            return principal.Claims.ToList();
        }

        /// <summary>
        /// 刷新令牌
        /// </summary>
        /// <param name="accessToken">令牌</param>
        /// <returns>令牌信息</returns>
        public static TokenInfo RefreshToken(string accessToken)
        {
            var claims = AnalysisToken(accessToken);

            return GenerateToken(claims);
        }
    }
}
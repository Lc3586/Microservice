using Business.Utils.Log;
using Microservice.Library.ConsoleTool;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Model.Utils.Config;
using Model.Utils.Log;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Configures
{
    /// <summary>
    /// JWT配置类
    /// </summary>
    public static class JWTConfigura
    {
        /// <summary>
        /// 注册JWT服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        public static IServiceCollection RegisterJWT(this IServiceCollection services, SystemConfig config)
        {
            "注册JWT服务.".ConsoleWrite();

            services.AddControllers(options =>
            {
                //全局身份（登录）验证, 除非添加AllowAnonymousAttribute特性忽略验证
                var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });

            AuthenticationBuilder builder;

            if (config.EnableCAS || config.EnableSampleAuthentication)
            {
                builder = services.AddAuthentication("JOC")
                                .AddPolicyScheme("JOC", "Jwt or Cookie", options =>
                                    {
                                        options.ForwardDefaultSelector = context =>
                                        {
                                            var bearerAuth = context.Request.Headers["Authorization"].FirstOrDefault()?.StartsWith("Bearer ") ?? false;
                                            if (bearerAuth)
                                                return JwtBearerDefaults.AuthenticationScheme;
                                            else
                                                return CookieAuthenticationDefaults.AuthenticationScheme;
                                        };
                                    });
            }
            else
                builder = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);

            builder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config.JWT.SecurityKey)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = config.JWT.Issuer,
                    ValidAudience = config.JWT.Audience,
                    ValidAlgorithms = new List<string> { config.JWT.Algorithm }
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {

                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        context.Request.Headers.TryGetValue("Authorization", out StringValues value);

                        Logger.Log(
                            NLog.LogLevel.Trace,
                            LogType.系统跟踪,
                            "JWT-AuthenticationFailed",
                            value,
                            context.Exception,
                            false);

                        return Task.CompletedTask;
                    }
                };
            });

            return services;
        }

        /// <summary>
        /// 配置JWT服务
        /// 注：方法在UseEndpoints之前调用
        /// </summary>
        /// <param name="app"></param>
        /// <param name="config"></param>
        public static IApplicationBuilder ConfiguraJWT(this IApplicationBuilder app, SystemConfig config)
        {
            "配置JWT服务.".ConsoleWrite();

            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }
    }
}

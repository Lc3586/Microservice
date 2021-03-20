using Business.Hub;
using Business.Utils.AuthorizePolicy;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Model.Utils.Config;
using System;

namespace Api.Configures
{
    /// <summary>
    /// 配置SignalR服务
    /// </summary>
    public static class SignalRConfigura
    {
        /// <summary>
        /// 注册SignalR服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterSignalR(this IServiceCollection services, SystemConfig config)
        {
            services.AddAuthorization(options =>
                {
                    options.AddPolicy(nameof(HubMethodAuthorizeRequirement), policy =>
                    {
                        policy.Requirements.Add(new HubMethodAuthorizeRequirement());
                    });
                })
                .AddSignalR()
                .AddHubOptions<LogHub>(option =>
                {
                    option.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
                    option.EnableDetailedErrors = config.RunMode != RunMode.Publish && config.RunMode != RunMode.Publish_Swagger;
                })
                .AddHubOptions<WeChatServiceHub>(option =>
                {
                    option.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
                    option.EnableDetailedErrors = config.RunMode != RunMode.Publish && config.RunMode != RunMode.Publish_Swagger;
                });

            //日志转发
            services.AddSingleton(new LogForward());

            return services;
        }

        /// <summary>
        /// 配置SignalR服务
        /// </summary>
        /// <param name="app"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IApplicationBuilder ConfiguraSignalR(this IApplicationBuilder app, SystemConfig config)
        {
            if (config.SignalrCors)
                app.UseCors("CorsPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<LogHub>("/loghub");
                endpoints.MapHub<WeChatServiceHub>("/wechathub");
            });

            //启动日志转发
            app.ApplicationServices.GetService<LogForward>().Start();

            return app;
        }
    }
}

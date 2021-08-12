using Business.Handler;
using Business.Hub;
using Business.Utils.Authorization;
using Microservice.Library.ConsoleTool;
using Microsoft.AspNetCore.Authorization;
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
            "注册SignalR服务.".ConsoleWrite();

            var builder = services.AddAuthorization(options =>
                 {
                     options.AddPolicy(nameof(SignalRHubRequirement), policy =>
                     {
                         policy.Requirements.Add(new SignalRHubRequirement());
                     });
                 }).AddScoped<IAuthorizationHandler, SignalRHubHandler<SignalRHubRequirement>>()
                 .AddSignalR()
                 .AddJsonProtocol(option =>
                 {
                     option.PayloadSerializerOptions.PropertyNamingPolicy = null;
                 })
                 .AddHubOptions<LogHub>(option =>
                 {
                     option.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
                     option.EnableDetailedErrors = config.RunMode != RunMode.Publish && config.RunMode != RunMode.Publish_Swagger;
                 });

            if (config.EnableWeChatService)
                builder.AddHubOptions<WeChatServiceHub>(option =>
                {
                    option.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
                    option.EnableDetailedErrors = config.RunMode != RunMode.Publish && config.RunMode != RunMode.Publish_Swagger;
                });

            if (config.EnableCAGC)
                builder.AddHubOptions<CAGCHub>(option =>
                {
                    option.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
                    option.EnableDetailedErrors = config.RunMode != RunMode.Publish && config.RunMode != RunMode.Publish_Swagger;
                });

            if (config.EnableUploadLargeFile)
                builder.AddHubOptions<ChunkFileMergeTaskHub>(option =>
                {
                    option.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
                    option.EnableDetailedErrors = config.RunMode != RunMode.Publish && config.RunMode != RunMode.Publish_Swagger;
                });

            $"初始化{LogForwardHandler.Name}.".ConsoleWrite();
            services.AddSingleton(new LogForwardHandler());

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
            "配置SignalR服务.".ConsoleWrite();

            if (config.SignalrCors)
                app.UseCors("CorsPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<LogHub>("/loghub");
                if (config.EnableWeChatService)
                    endpoints.MapHub<WeChatServiceHub>("/wechathub");
                if (config.EnableCAGC)
                    endpoints.MapHub<CAGCHub>("/cagchub");
                if (config.EnableUploadLargeFile)
                    endpoints.MapHub<ChunkFileMergeTaskHub>("/cfmthub");
            });

            $"启动{LogForwardHandler.Name}.".ConsoleWrite();
            app.ApplicationServices.GetService<LogForwardHandler>().Start();

            return app;
        }
    }
}

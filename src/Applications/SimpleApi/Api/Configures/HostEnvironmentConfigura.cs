using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Model.Utils.Config;

namespace Api.Configures
{
    /// <summary>
    /// 主机环境配置类
    /// </summary>
    public static class HostEnvironmentConfigura
    {
        /// <summary>
        /// 注册主机环境
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <remarks></remarks>
        public static IServiceCollection RegisterHostEnvironment(this IServiceCollection services, SystemConfig config)
        {
            //services.AddSingleton();

            return services;
        }

        /// <summary>
        /// 配置主机环境
        /// </summary>
        /// <param name="app"></param>
        /// <param name="webHostEnvironment"></param>
        /// <param name="hostingEnvironment"></param>
        /// <param name="config"></param>
        /// <remarks></remarks>
#pragma warning disable CS0618 // 类型或成员已过时
        public static IApplicationBuilder ConfiguraHostEnvironment(this IApplicationBuilder app, IWebHostEnvironment webHostEnvironment, IHostingEnvironment hostingEnvironment, SystemConfig config)
#pragma warning restore CS0618 // 类型或成员已过时
        {
            webHostEnvironment.ApplicationName = config.ProjectName;
            if (webHostEnvironment.EnvironmentName != "Development")
                webHostEnvironment.ContentRootPath = config.AbsoluteStorageDirectory;

            hostingEnvironment.ApplicationName = config.ProjectName;
            if (hostingEnvironment.EnvironmentName != "Development")
                hostingEnvironment.ContentRootPath = config.AbsoluteStorageDirectory;

            return app;
        }
    }
}

using Microservice.Library.Extension;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Model.Utils.Config;
using System;
using System.IO;

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
            if (webHostEnvironment.EnvironmentName == "Development")
            {
                config._AbsoluteStorageDirectory = webHostEnvironment.ContentRootPath;
                config._AbsoluteWWWRootDirectory = webHostEnvironment.WebRootPath;
            }
            else
            {
                //转移静态文件目录
                Copy(webHostEnvironment.WebRootPath, config.AbsoluteWWWRootDirectory);
                webHostEnvironment.ContentRootPath = config.AbsoluteStorageDirectory;
                webHostEnvironment.WebRootPath = config.AbsoluteWWWRootDirectory;
            }

            hostingEnvironment.ApplicationName = config.ProjectName;
            if (hostingEnvironment.EnvironmentName == "Development")
            {
                config._AbsoluteStorageDirectory = hostingEnvironment.ContentRootPath;
                config._AbsoluteWWWRootDirectory = hostingEnvironment.WebRootPath;
            }
            else
            {
                hostingEnvironment.ContentRootPath = config.AbsoluteStorageDirectory;
                hostingEnvironment.WebRootPath = config.AbsoluteWWWRootDirectory;
            }

            return app;
        }

        /// <summary>
        /// 复制文件
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        static void Copy(string source, string destination)
        {
            var sourceDir = new DirectoryInfo(source);
            if (!sourceDir.Exists)
                return;

            sourceDir.GetFiles().ForEach(file =>
            {
                var dir = file.DirectoryName.Replace(source, destination);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                File.Copy(file.FullName, file.FullName.Replace(source, destination), true);
            });

            sourceDir.GetDirectories().ForEach(dir =>
            {
                Copy(dir.FullName, dir.FullName.Replace(source, destination));
            });
        }
    }
}

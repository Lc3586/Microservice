using Microservice.Library.ConsoleTool;
using Microservice.Library.Extension;
using Microservice.Library.File;
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
            "配置主机环境.".ConsoleWrite();

            webHostEnvironment.ApplicationName = config.SystemName;

            $"当前为{webHostEnvironment.EnvironmentName}环境.".ConsoleWrite();

            if (webHostEnvironment.EnvironmentName == "Development")
            {
                //config.AbsoluteStorageDirectory = webHostEnvironment.ContentRootPath;
                config.AbsoluteWWWRootDirectory = webHostEnvironment.WebRootPath;
            }
            else
            {
                //转移静态文件目录
                if (webHostEnvironment.WebRootPath?.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).ToUpperInvariant()
                    != config.AbsoluteWWWRootDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).ToUpperInvariant()
                        && Directory.Exists(webHostEnvironment.WebRootPath))
                    webHostEnvironment.WebRootPath.CopyTo(config.AbsoluteWWWRootDirectory, true, true);

                webHostEnvironment.ContentRootPath = config.AbsoluteStorageDirectory;
                webHostEnvironment.WebRootPath = config.AbsoluteWWWRootDirectory;
            }

            hostingEnvironment.ApplicationName = config.SystemName;
            if (hostingEnvironment.EnvironmentName != "Development")
            {
                hostingEnvironment.ContentRootPath = config.AbsoluteStorageDirectory;
                hostingEnvironment.WebRootPath = config.AbsoluteWWWRootDirectory;
            }

            $"ContentRootPath(应用程序文件存储根目录) => {webHostEnvironment.ContentRootPath}.".ConsoleWrite();
            $"WebRootPath(应用程序站点文件根目录) => {webHostEnvironment.WebRootPath}.".ConsoleWrite();

            return app;
        }
    }
}

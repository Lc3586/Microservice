using Microservice.Library.Extension;
using Microservice.Library.File;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Model.Utils.Config;
using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace Api.Configures
{
    /// <summary>
    /// 应用程序保护数据配置类
    /// </summary>
    public static class DataProtectionConfigura
    {
        /// <summary>
        /// 注册应用程序保护数据配置
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <remarks></remarks>
        public static IServiceCollection RegisterDataProtection(this IServiceCollection services, SystemConfig config)
        {
            var dpBuilder = services.AddDataProtection();

            if (!config.AbsoluteDataProtectionDirectory.IsNullOrWhiteSpace())
                dpBuilder.PersistKeysToFileSystem(new DirectoryInfo(config.AbsoluteDataProtectionDirectory));

            if (!config.DataProtectionCertificateFile.IsNullOrWhiteSpace())
            {
                if (!File.Exists(config.AbsoluteDataProtectionCertificateFile))
                    throw new ApplicationException($"指定的秘钥文件({config.AbsoluteDataProtectionCertificateFile})不存在.");

                Console.WriteLine(config.DataProtectionCertificateFilePassword);
                var cert = new X509Certificate2(config.AbsoluteDataProtectionCertificateFile, config.DataProtectionCertificateFilePassword);
                dpBuilder.ProtectKeysWithCertificate(cert);
            }

            return services;
        }
    }
}

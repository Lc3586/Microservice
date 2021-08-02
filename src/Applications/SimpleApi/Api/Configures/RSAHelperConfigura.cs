using Microservice.Library.ConsoleTool;
using Microservice.Library.Extension.Helper;
using Microsoft.Extensions.DependencyInjection;
using Model.Utils.Config;

namespace Api.Configures
{
    /// <summary>
    /// RSA帮助类配置类
    /// </summary>
    public static class RSAHelperConfigura
    {
        /// <summary>
        /// 注册RSA帮助类
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        public static IServiceCollection RegisterRSAHelper(this IServiceCollection services, SystemConfig config)
        {
            "注册RSA帮助类.".ConsoleWrite();

            RSAHelper rsaHelper = config.RSA.UseCertFile
                    ? new RSAHelper(config.RSA.PemFilePath, config.RSA.CertFilePath, config.RSA.CertPassword)
                    : new RSAHelper(config.RSA.PublicKey, config.RSA.PrivateKey);

            services.AddSingleton(rsaHelper);

            return services;
        }
    }
}

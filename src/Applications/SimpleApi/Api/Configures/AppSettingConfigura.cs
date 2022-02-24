using Microservice.Library.ConsoleTool;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Model.Utils.Config;

namespace Api.Configures
{
    /// <summary>
    /// 应用配置类
    /// </summary>
    public static class AppSettingConfigura
    {
        /// <summary>
        /// 注册应用配置热重载服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static IServiceCollection RegisterAppSettingReload(this IServiceCollection services, IConfiguration configuration)
        {
            "注册接口版本服务.".ConsoleWrite();

            services.Configure<SystemConfig>(configuration.GetSection("Learning"));

            return services;
        }
    }
}

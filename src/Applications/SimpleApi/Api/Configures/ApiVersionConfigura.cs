using Microservice.Library.ConsoleTool;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Model.Utils.Config;

namespace Api.Configures
{
    /// <summary>
    /// 接口版本配置类
    /// </summary>
    public static class ApiVersionConfigura
    {
        /// <summary>
        /// 注册接口版本服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        public static IServiceCollection RegisterApiVersion(this IServiceCollection services, SystemConfig config)
        {
            "注册接口版本服务.".ConsoleWrite();

            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;

                //通过Header或QueryString设置版本
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new HeaderApiVersionReader(config.ApiVersion.Keyword),
                    new QueryStringApiVersionReader(config.ApiVersion.Keyword));

                //请求时如果未指定版本则使用默认版本
                if (config.ApiVersion.DefaultVersion.Count == 2)
                {
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.DefaultApiVersion = new ApiVersion(config.ApiVersion.DefaultVersion[0], config.ApiVersion.DefaultVersion[1]);
                }

                //请求时默认指定为当前最高版本
                if (config.ApiVersion.SelectMaximumVersion)
                    options.ApiVersionSelector = new CurrentImplementationApiVersionSelector(options);
            });

            services.AddVersionedApiExplorer();

            return services;
        }
    }
}

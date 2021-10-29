using Business.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Model.Utils.Config;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;

namespace Api.Configures
{
    /// <summary>
    /// 配置Swagger选项
    /// </summary>
    /// <remarks>当Api有多个版本时使用此类,将系统配置应用于Swagger配置</remarks>
    public class ConfigureSwaggerGenOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider ApiVersionProvider;

        readonly SwaggerApiOptions Options;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiVersionProvider"></param>
        /// <param name="options"></param>
        public ConfigureSwaggerGenOptions(
           IApiVersionDescriptionProvider apiVersionProvider,
           IOptions<SwaggerApiOptions> options)
        {
            ApiVersionProvider = apiVersionProvider;
            Options = options.Value;
        }

        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="options"></param>
        public void Configure(SwaggerGenOptions options)
        {
            //分组
            options.DocInclusionPredicate((string documentName, ApiDescription apiDescription) =>
            {
                if (apiDescription.GroupName == null)
                    return true;

                //if (apiDescription.GroupName == "测试")
                //{
                //    var a = 1;
                //}

                var type = typeof(ApiVersionAttribute);

                var apiVersions = apiDescription.ActionDescriptor.EndpointMetadata.Where(o => o.GetType() == type);
                if (apiVersions.Any())
                {
                    var versions = apiVersions.SelectMany(o => ((ApiVersionAttribute)o).Versions);
                    return apiDescription.GroupName.Split(',').Any(o => versions.Any(p => documentName == $"{p} {o}"));
                }
                else
                    return apiDescription.GroupName.Split(',').Any(o => documentName == $"{SwaggerApiVersion.NotConfigured.Version} {o}");
            });

            foreach (var description in ApiVersionProvider.ApiVersionDescriptions)
            {
                var version = description.ApiVersion.ToString();

                var apiVersion = Options.ApiVersions.FirstOrDefault(o => o.Version == version);

                if (apiVersion == null)
                {
                    apiVersion = SwaggerApiVersion.NotConfigured;
                    apiVersion.Version = version;
                }

                //分组
                var groups = Options.Groups.Where(o => o.Versions.Contains(apiVersion.Version));
                foreach (var group in groups)
                {
                    options.SwaggerDoc(
                        $"{apiVersion.Version} {group.Name}",
                        new OpenApiInfo()
                        {
                            Title = $"{apiVersion.Title} {group.Title}",
                            Version = apiVersion.Version,
                            Description = $"{apiVersion.Description} {group.Description}"
                        });
                }
            }
        }
    }
}

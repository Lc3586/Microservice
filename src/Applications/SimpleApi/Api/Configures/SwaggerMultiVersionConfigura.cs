﻿using Microservice.Library.ConsoleTool;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Model.Utils.Config;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Api.Configures
{
    /// <summary>
    /// Swagger多版本文档配置类
    /// </summary>
    public static class SwaggerMultiVersionConfigura
    {
        /// <summary>
        /// 注册Swagger多版本文档服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        public static IServiceCollection RegisterSwaggerMultiVersion(this IServiceCollection services, SystemConfig config)
        {
            "注册Swagger多版本文档服务.".ConsoleWrite();

            #region 多版本文档

            services.AddApiVersioning();
            services.AddVersionedApiExplorer();

            #endregion

            services.AddSwaggerGen(s =>
            {
                #region 配置文档

                //多版本文档配置
                //注册自定义配置程序，将系统配置（SystemConfig）应用于Swagger配置（AutoMapperGeneratorOptions）。
                services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerGenOptions>();
                services.AddTransient(s => s.GetRequiredService<IOptions<SwaggerApiMultiVersionDescriptionOptions>>().Value);
                services.Configure<SwaggerApiMultiVersionDescriptionOptions>(options =>
                {
                    options.ApiMultiVersionDescription = config.Swagger.ApiMultiVersion;
                });

                #endregion

                #region 自定义架构Id选择器

                static string SchemaIdSelector(Type modelType)
                {
                    if (!modelType.IsConstructedGenericType) return modelType.FullName.Replace("[]", "Array");

                    var prefix = modelType.GetGenericArguments()
                        .Select(genericArg => SchemaIdSelector(genericArg))
                        .Aggregate((previous, current) => previous + current);

                    return prefix + modelType.FullName.Split('`').First();
                }

                s.CustomSchemaIds(SchemaIdSelector);

                #endregion

                s.SchemaFilter<OpenApiSchemaFilter>();

                #region 为JSON文件和UI设置xml文档路径

                var basePath = AppContext.BaseDirectory;
                foreach (var item in config.Swagger.XmlComments)
                {
                    var xmlPath = Path.Combine(basePath, item);

                    if (File.Exists(xmlPath))
                        s.IncludeXmlComments(xmlPath);
                }

                #endregion

                //启用注解
                s.EnableAnnotations();
            })
            .AddMvc()
            //禁用框架结构属性小驼峰命名规则
            .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

            return services;
        }

        /// <summary>
        /// 配置Swagger多版本文档服务
        /// </summary>
        /// <param name="app"></param>
        /// <param name="config"></param>
        public static IApplicationBuilder ConfiguraSwaggerMultiVersion(this IApplicationBuilder app, SystemConfig config)
        {
            "配置Swagger多版本文档服务.".ConsoleWrite();

            var apiVersionDescription = app.ApplicationServices.GetService<IApiVersionDescriptionProvider>();

            #region 方言配置（展示用，普通项目无需添加此内容）

            var supportedCultures = new[]
            {
                new CultureInfo("zh-CN"),
                new CultureInfo("en-US"),
                new CultureInfo("es-ES"),
                //new CultureInfo("fr"),
                //new CultureInfo("sv-SE"),
            };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                //默认方言
                DefaultRequestCulture = new RequestCulture("zh-CN"),
                // Formatting numbers, dates, etc.
                SupportedCultures = supportedCultures,
                // UI strings that we have localized.
                SupportedUICultures = supportedCultures
            });

            #endregion

            app.UseSwagger(s =>
            {
                s.PreSerializeFilters.Add((swagger, httpReq) =>
                {
                    swagger.Servers = new List<OpenApiServer> {
                        new OpenApiServer {
                            Url = $"{httpReq.Scheme}://{httpReq.Host.Value}",
                            Description = "当前地址"
                        },
                        new OpenApiServer {
                            Url = config.PublishRootUrl,
                            Description = "服务器地址"
                        }
                    };
                });
            });
            app.UseSwaggerUI(s =>
            {
                //多版本文档
                foreach (var description in apiVersionDescription.ApiVersionDescriptions)
                {
                    s.SwaggerEndpoint($"{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }
                s.SwaggerEndpoint("/swagger/v1.0/swagger.json", "v1.0 文档");

                #region 页面自定义选项

                s.DocumentTitle = $"{config.ProjectName}接口文档";//页面标题
                s.DisplayOperationId();//显示操作Id
                s.DisplayRequestDuration();//显示请求持续时间
                s.EnableFilter();//启用顶部筛选框

                //注入自定义文件
                var dirPath = Path.Combine(config.AbsoluteWWWRootDirectory, "swagger");
                InjectFileFromDir(new DirectoryInfo(dirPath));
                void InjectFileFromDir(DirectoryInfo dir)
                {
                    foreach (var file in dir.GetFiles())
                    {
                        if (file.Name.Contains("casLogin"))
                        {
                            //cas登录脚本脚本
                            if (!config.EnableCAS)
                                continue;
                        }
                        else if (file.Name.Contains("saLogin"))
                        {
                            //sa登录脚本脚本
                            if (!config.EnableSampleAuthentication)
                                continue;
                        }

                        string key = string.Empty;
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                            key = "/swagger/";
                        else
                            key = "\\swagger\\";

                        var index = file.FullName.IndexOf(key) + 1;

                        if (file.Extension == ".js")
                            s.InjectJavascript(file.FullName[index..]);
                        else if (file.Extension == ".css")
                            s.InjectStylesheet(file.FullName[index..]);
                    }

                    foreach (var innerDir in dir.GetDirectories())
                    {
                        if (innerDir.Name.Contains("ignore"))
                            continue;
                        InjectFileFromDir(innerDir);
                    }
                }

                #endregion

                s.RoutePrefix = string.Empty;
            });

            return app;
        }
    }
}

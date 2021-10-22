using Microservice.Library.ConsoleTool;
using Microservice.Library.Extension;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Model.Utils.Config;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Encodings.Web;

namespace Api.Configures
{
    /// <summary>
    /// Swagger单文档配置类
    /// </summary>
    public static class SwaggerConfigura
    {
        /// <summary>
        /// 注册Swagger服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        public static IServiceCollection RegisterSwagger(this IServiceCollection services, SystemConfig config)
        {
            "注册Swagger服务.".ConsoleWrite();

            services.AddMvc()
                //禁用框架结构属性小驼峰命名规则
                .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

            if (config.Swagger.Groups.Any_Ex())
                services.AddVersionedApiExplorer();

            services.AddSwaggerGen(options =>
            {
                #region 配置文档

                var apiVersion = config.Swagger.ApiVersions.FirstOrDefault() ?? SwaggerApiVersion.NotConfigured;

                //分组
                options.DocInclusionPredicate((string documentName, ApiDescription apiDescription) =>
                {
                    return apiDescription.GroupName == null || apiDescription.GroupName.Split(',').Contains(documentName);
                });

                var groups = config.Swagger.Groups.Where(o => o.Versions.Contains(apiVersion.Version));
                foreach (var group in groups)
                {
                    options.SwaggerDoc(
                        group.Name,
                        new OpenApiInfo()
                        {
                            Title = $"{apiVersion.Title} {group.Title}",
                            Version = apiVersion.Version,
                            Description = $"{apiVersion.Description} {group.Description}"
                        });
                }

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

                options.CustomSchemaIds(SchemaIdSelector);

                #endregion

                options.SchemaFilter<OpenApiSchemaFilter>();

                options.DocumentFilter<DocumentFilter>();

                #region 为JSON文件和UI设置xml文档路径

                //获取应用程序所在目录（绝对，不受工作目录影响，建议采用此方法获取路径）
                var basePath = AppContext.BaseDirectory;
                foreach (var item in config.Swagger.XmlComments)
                {
                    var xmlPath = Path.Combine(basePath, item);

                    if (File.Exists(xmlPath))
                        options.IncludeXmlComments(xmlPath, true);
                }

                #endregion

                //启用注解
                options.EnableAnnotations();

                if (config.EnableJWT)
                {
                    options.AddSecurityDefinition("JWT", new OpenApiSecurityScheme
                    {
                        Description = "在请求的Header中，添加\"Authorization\":\"Bearer xxxxxxxxxxxxxx\"",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        Scheme = "Bearer"
                    });

                    var scheme = new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "JWT" }
                    };

                    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        [scheme] = Array.Empty<string>()
                    });
                }
            });

            return services;
        }

        /// <summary>
        /// 配置Swagger服务
        /// </summary>
        /// <param name="app"></param>
        /// <param name="config"></param>
        public static IApplicationBuilder ConfiguraSwagger(this IApplicationBuilder app, SystemConfig config)
        {
            "配置Swagger服务.".ConsoleWrite();

            #region 用户语言（展示用，普通项目无需添加此内容）

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
                            Url =config.WebRootUrlMatchScheme(httpReq.Scheme),
                            Description = "当前地址"
                        },
                        new OpenApiServer {
                            Url = config.WebRootUrlMatchScheme(httpReq.Scheme,true),
                            Description = "其他地址"
                        }
                    };
                });
            });
            app.UseSwaggerUI(s =>
            {
                var apiVersion = config.Swagger.ApiVersions.FirstOrDefault() ?? SwaggerApiVersion.NotConfigured;

                //分组
                var groups = config.Swagger.Groups.Where(o => o.Versions.Contains(apiVersion.Version));
                foreach (var group in groups)
                {
                    s.SwaggerEndpoint($"swagger/{UrlEncoder.Default.Encode(group.Name)}/swagger.json", $"{apiVersion.Name} {group.Name}");
                }

                #region 页面自定义选项

                s.DocumentTitle = apiVersion.Title;//页面标题
                s.DisplayOperationId();//显示操作Id
                s.DisplayRequestDuration();//显示请求持续时间
                s.EnableFilter();//启用顶部筛选框

                //注入自定义文件
                var dirPath = Path.Combine(config.AbsoluteWWWRootDirectory, "swagger");
                InjectFileFromDir(new DirectoryInfo(dirPath));
                void InjectFileFromDir(DirectoryInfo dir)
                {
                    if (!dir.Exists)
                        return;

                    foreach (var file in dir.GetFiles())
                    {
                        if (file.Name.Contains("casLogin"))
                        {
                            //cas登录脚本
                            if (!config.EnableCAS)
                                continue;
                        }
                        else if (file.Name.Contains("saLogin"))
                        {
                            //sa登录脚本
                            if (!config.EnableSampleAuthentication)
                                continue;
                        }
                        else if (file.Name.Contains("wechat"))
                        {
                            //微信脚本
                            if (!config.EnableWeChatService)
                                continue;
                        }
                        else if (file.Name.Contains("Site"))
                        {
                            //站点脚本
                            if (config.EnableSite?.Any(o => file.Name.Contains(o)) != true)
                                continue;
                        }
                        else if (file.Name.Contains("jwt"))
                        {
                            //jwt脚本
                            if (!config.EnableJWT)
                                continue;
                        }
                        else if (file.Name.Contains("cagc"))
                        {
                            //自动生成代码应用程序脚本
                            if (!config.EnableCAGC)
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

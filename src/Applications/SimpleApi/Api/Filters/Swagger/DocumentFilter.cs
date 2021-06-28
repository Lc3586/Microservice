using Business.Utils;
using Business.Utils.Log;
using Entity.System;
using Microservice.Library.Container;
using Microservice.Library.Extension;
using Microservice.Library.FreeSql.Gen;
using Microsoft.OpenApi.Models;
using Model.System;
using Model.Utils.Config;
using Model.Utils.Log;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Api
{
    /// <summary>
    /// 接口文档过滤器
    /// </summary>
    /// <remarks>LCTR 2021-02-21</remarks>
    public class DocumentFilter : IDocumentFilter
    {
        static SystemConfig Config => AutofacHelper.GetService<SystemConfig>();

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            InitResourcesData(swaggerDoc);

            var removePaths = new List<string>();

            if (!Config.EnableCAS)
            {
                removePaths.AddRange(context.ApiDescriptions.Where(o => o.RelativePath.IndexOf("cas/") == 0)
                    .Select(o => o.RelativePath));
            }

            if (!Config.EnableSampleAuthentication)
            {
                removePaths.AddRange(context.ApiDescriptions.Where(o => o.RelativePath.IndexOf("sa/") == 0)
                    .Select(o => o.RelativePath));
            }

            if (!Config.EnableWeChatService)
            {
                removePaths.AddRange(context.ApiDescriptions.Where(o => o.RelativePath.IndexOf("wechat-user/") == 0
                                                                        || o.RelativePath.IndexOf("wechat-oath/") == 0)
                    .Select(o => o.RelativePath));
            }

            if (!Config.EnableJWT)
            {
                removePaths.AddRange(context.ApiDescriptions.Where(o => o.RelativePath.IndexOf("jwt/") == 0)
                    .Select(o => o.RelativePath));
            }

            if (!Config.EnableCAGC)
            {
                removePaths.AddRange(context.ApiDescriptions.Where(o => o.RelativePath.IndexOf("cagc/") == 0)
                    .Select(o => o.RelativePath));
            }

            if (removePaths.Any())
                removePaths.ForEach(o => swaggerDoc.Paths.Remove($"/{o}"));
        }

        /// <summary>
        /// 初始化资源数据
        /// </summary>
        static async void InitResourcesData(OpenApiDocument swaggerDoc)
        {
            try
            {
                var orm = AutofacHelper.GetService<IFreeSqlProvider>()
                    .GetFreeSql();

                var newData = new List<System_Resources>();
                swaggerDoc.Paths.ForEach(p =>
                {
                    var uri = Regex.Replace(p.Key, @"{(.*?)}", "%");
                    var resources = orm.Select<System_Resources>()
                            .Where(o => o.Uri == uri && o.Type == ResourcesType.接口)
                            .ToOne(o => new { o.Id, o.Uri });

                    if (resources != default)
                        return;

                    p.Value.Operations.ForEach(o =>
                    {
                        newData.Add(new System_Resources
                        {
                            Name = $"{string.Join("", o.Value.Tags.Select(t => $"[{t.Name}]"))}<{o.Key}>{o.Value.Summary}",
                            Code = p.Key.ToMD5String(),
                            Type = ResourcesType.接口,
                            Uri = uri,
                            Enable = true,
                            Remark = "通过Swagger接口文档自动生成."
                        }.InitEntity());

                    });
                });

                if (newData.Any() && await orm.Insert(newData).ExecuteAffrowsAsync() <= 0)
                    throw new MessageException($"新增数据失败, 共{newData.Count}条数据.");
            }
            catch (Exception ex)
            {
                Logger.Log(
                    NLog.LogLevel.Error,
                    LogType.系统异常,
                    "初始化资源数据失败.",
                    null,
                    ex,
                    false);
            }
        }
    }
}

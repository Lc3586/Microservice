using Business.Utils.Log;
using Microservice.Library.ConsoleTool;
using Microservice.Library.FreeSql.Gen;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Model.Utils.Config;
using Model.Utils.Log;
using MySqlConnector;

namespace Api.Configures
{
    /// <summary>
    /// FreeSql配置类
    /// </summary>
    public static class FreeSqlConfigura
    {
        /// <summary>
        /// 注册FreeSql服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        public static IServiceCollection RegisterFreeSql(this IServiceCollection services, SystemConfig config)
        {
            "注册FreeSql服务.".ConsoleWrite();
            $"使用{config.Database.DatabaseType}数据库.".ConsoleWrite();

            services.AddFreeSql(options =>
            {
                options.FreeSqlGeneratorOptions.ConnectionString = config.Database.ConnectString;
                options.FreeSqlGeneratorOptions.DatabaseType = config.Database.DatabaseType;
                options.FreeSqlGeneratorOptions.LazyLoading = true;
                options.FreeSqlGeneratorOptions.MonitorCommandExecuting = (cmd) =>
                {
                    Logger.Log(
                        NLog.LogLevel.Trace,
                        LogType.系统跟踪,
                        "FreeSql-MonitorCommandExecuting",
                        cmd.CommandText,
                        null,
                        false);
                };
                options.FreeSqlGeneratorOptions.MonitorCommandExecuted = (cmd, log) =>
                {
                    Logger.Log(
                        NLog.LogLevel.Trace,
                        LogType.系统跟踪,
                        "FreeSql-MonitorCommandExecuted",
                         $"命令: {cmd.CommandText},\r\n日志: {log}.",
                        null,
                        false);
                };
                options.FreeSqlGeneratorOptions.HandleCommandLog = (content) =>
                {
                    Logger.Log(
                        NLog.LogLevel.Trace,
                        LogType.系统跟踪,
                        "FreeSql-HandleCommandLog",
                        content,
                        null,
                        false);
                };

                options.FreeSqlDevOptions.AutoSyncStructure = config.FreeSql.AutoSyncStructure;
                options.FreeSqlDevOptions.SyncStructureNameConvert = config.FreeSql.SyncStructureNameConvert;
                options.FreeSqlDevOptions.SyncStructureOnStartup = config.FreeSql.SyncStructureOnStartup;

                options.FreeSqlDbContextOptions.EnableAddOrUpdateNavigateList = true;
                options.FreeSqlDbContextOptions.EntityAssembly = config.Database.EntityAssembly;
            });

            return services;
        }

        /// <summary>
        /// 配置FreeSql服务
        /// </summary>
        /// <param name="app"></param>
        /// <param name="config"></param>
        public static IApplicationBuilder ConfiguraFreeSql(this IApplicationBuilder app, SystemConfig config)
        {
            "配置FreeSql服务.".ConsoleWrite();

            //单库预热
            app.ApplicationServices
               .GetService<IFreeSqlProvider>()
               .GetFreeSql();

            return app;
        }
    }
}

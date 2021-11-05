using DataMigration.Application.Log;
using DataMigration.Application.Model;
using Microservice.Library.ConsoleTool;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DataMigration.Application.Configures
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
        /// <param name="config">配置</param>
        public static IServiceCollection RegisterFreeSql(this IServiceCollection services, Config config)
        {
            services.AddFreeSql<int>(options =>
             {
                 options
                    //设置空闲时间
                    .SetUpIdle(TimeSpan.FromMinutes(30))
                    //监听通知
                    .SetUpNotice((arg) =>
                    {
                        var text = $"Type : \"{arg.NoticeType}\", Key : \"{arg.Key}\", Log : \"{arg.Log}\"";

                        Logger.Log(
                            NLog.LogLevel.Trace,
                            LogType.系统跟踪,
                            "FreeSql-IdleBus-Notice",
                            text,
                            arg.Exception);
                    });

                 //设置生成配置
                 $"使用{config.SourceDataType}数据库.\r\n".ConsoleWrite();

                 options.Add(0, options =>
                 {
                     options.FreeSqlGeneratorOptions.ConnectionString = config.SourceConnectingString;
                     options.FreeSqlGeneratorOptions.DatabaseType = config.SourceDataType;
                     options.FreeSqlGeneratorOptions.LazyLoading = true;

                     options.FreeSqlGeneratorOptions.MonitorCommandExecuting = (cmd) =>
                     {
                         Logger.Log(
                             NLog.LogLevel.Trace,
                             LogType.系统跟踪,
                             "Source-FreeSql-MonitorCommandExecuting",
                             cmd.CommandText);
                     };
                     options.FreeSqlGeneratorOptions.MonitorCommandExecuted = (cmd, log) =>
                     {
                         Logger.Log(
                             NLog.LogLevel.Trace,
                             LogType.系统跟踪,
                             "Source-FreeSql-MonitorCommandExecuted",
                             $"命令: {cmd.CommandText},\r\n日志: {log}.");
                     };
                     options.FreeSqlGeneratorOptions.HandleCommandLog = (content) =>
                     {
                         Logger.Log(
                             NLog.LogLevel.Trace,
                             LogType.系统跟踪,
                             "Source-FreeSql-MonitorCommandExecuting",
                             content);
                     };

                     options.FreeSqlDevOptions.AutoSyncStructure = false;
                     options.FreeSqlDevOptions.SyncStructureNameConvert = config.SyncStructureNameConvert;
                     options.FreeSqlDevOptions.SyncStructureOnStartup = false;

                     options.FreeSqlDbContextOptions.EnableAddOrUpdateNavigateList = true;
                     options.FreeSqlDbContextOptions.EntityAssemblyFiles = config.EntityAssemblyFiles;
                 });

                 options.Add(1, options =>
                 {
                     options.FreeSqlGeneratorOptions.ConnectionString = config.TargetConnectingString;
                     options.FreeSqlGeneratorOptions.DatabaseType = config.TargetDataType;
                     options.FreeSqlGeneratorOptions.LazyLoading = true;

                     options.FreeSqlGeneratorOptions.MonitorCommandExecuting = (cmd) =>
                     {
                         Logger.Log(
                             NLog.LogLevel.Trace,
                             LogType.系统跟踪,
                             "Target-FreeSql-MonitorCommandExecuting",
                             cmd.CommandText);
                     };
                     options.FreeSqlGeneratorOptions.MonitorCommandExecuted = (cmd, log) =>
                     {
                         Logger.Log(
                             NLog.LogLevel.Trace,
                             LogType.系统跟踪,
                             "Target-FreeSql-MonitorCommandExecuted",
                             $"命令: {cmd.CommandText},\r\n日志: {log}.");
                     };
                     options.FreeSqlGeneratorOptions.HandleCommandLog = (content) =>
                     {
                         Logger.Log(
                             NLog.LogLevel.Trace,
                             LogType.系统跟踪,
                             "Target-FreeSql-MonitorCommandExecuting",
                             content);
                     };

                     options.FreeSqlDevOptions.AutoSyncStructure = false;
                     options.FreeSqlDevOptions.SyncStructureNameConvert = config.SyncStructureNameConvert;
                     options.FreeSqlDevOptions.SyncStructureOnStartup = false;

                     options.FreeSqlDbContextOptions.EnableAddOrUpdateNavigateList = true;
                     options.FreeSqlDbContextOptions.EntityAssemblyFiles = config.EntityAssemblyFiles;
                 });
             });

            return services;
        }
    }
}

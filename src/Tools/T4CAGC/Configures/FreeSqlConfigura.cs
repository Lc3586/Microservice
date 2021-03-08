using FreeSql;
using Microsoft.Extensions.DependencyInjection;
using T4CAGC.Log;

namespace T4CAGC.Configures
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
        /// <param name="connectString">连接字符串</param>
        /// <param name="dataType">数据库类型</param>
        public static IServiceCollection RegisterFreeSql(this IServiceCollection services, string connectString, DataType dataType)
        {
            services.AddFreeSql(s =>
             {
                 s.FreeSqlGeneratorOptions.ConnectionString = connectString;
                 s.FreeSqlGeneratorOptions.DatabaseType = dataType;
                 s.FreeSqlGeneratorOptions.LazyLoading = true;
                 s.FreeSqlGeneratorOptions.MonitorCommandExecuting = (cmd) =>
                 {
                     Logger.Log(
                         NLog.LogLevel.Trace,
                         LogType.系统跟踪,
                         "FreeSql-MonitorCommandExecuting",
                         cmd.CommandText);
                 };
                 s.FreeSqlGeneratorOptions.MonitorCommandExecuted = (cmd, log) =>
                 {
                     Logger.Log(
                         NLog.LogLevel.Trace,
                         LogType.系统跟踪,
                         "FreeSql-MonitorCommandExecuted",
                          $"命令: {cmd.CommandText},\r\n日志: {log}.");
                 };
                 s.FreeSqlGeneratorOptions.HandleCommandLog = (content) =>
                 {
                     Logger.Log(
                         NLog.LogLevel.Trace,
                         LogType.系统跟踪,
                         "FreeSql-HandleCommandLog",
                         content);
                 };
             });

            return services;
        }
    }
}

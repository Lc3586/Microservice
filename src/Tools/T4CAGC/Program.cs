using Autofac;
using Autofac.Extensions.DependencyInjection;
using McMaster.Extensions.CommandLineUtils;
using Microservice.Library.Configuration;
using Microservice.Library.ConsoleTool;
using Microservice.Library.Container;
using Microservice.Library.Extension;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using T4CAGC.Configures;
using T4CAGC.Handler;
using T4CAGC.Log;
using T4CAGC.Model;

namespace T4CAGC
{
    [Command(Name = "generate", Description = "代码自动生成工具.")]
    [VersionOption("-v|--version", "v0.0.0.1-beta", Description = "版本信息.")]
    [HelpOption(Description = "帮助信息.")]
    class Program
    {
        static Task<int> Main(string[] args) => CommandLineApplication.ExecuteAsync<Program>(args);

        #region 参数

        //[Argument(1, Description = "覆盖已有文件（默认不覆盖）.")]
        //bool OverlayFile { get; } = false;

        #endregion

        #region 配置

        [Option("-c|--ConfigPath", Description = "配置文件路径: 不指定时使用默认配置.")]
        public string ConfigPath { get; } = "config/generateconfig.json";

        [Option("-l|--LoggerType", Description = "日志类型（默认Console）.")]
        public LoggerType LoggerType { get; } = LoggerType.Console;

        [Option("-s|--DataSource", Description = "数据源: CSV文件路径、数据库连接字符串.")]
        public string DataSource { get; }

        [Option("-t|--DataSourceType", Description = "数据源类型（默认CSV文件）.")]
        public DataSourceType DataSourceType { get; } = DataSourceType.CSV;

        [Option("-g|--GenType", Description = "生成类型（默认EnrichmentProject）.")]
        public GenType GenType { get; } = GenType.EnrichmentProject;

        [Option("-p|--OutputPath", Description = "输出路径")]
        public string OutputPath { get; }

        [Option("-o|--OverlayFile", Description = "覆盖已有文件（默认不覆盖）.")]
        public bool OverlayFile { get; } = false;

        #endregion

#pragma warning disable IDE0051 // 删除未使用的私有成员
#pragma warning disable IDE0060 // 删除未使用的参数
        async Task<int> OnExecuteAsync(CommandLineApplication app, CancellationToken cancellationToken = default)
#pragma warning restore IDE0060 // 删除未使用的参数
#pragma warning restore IDE0051 // 删除未使用的私有成员
        {
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                Console.OutputEncoding = Encoding.GetEncoding(936); //new UTF8Encoding(true);

                #region 欢迎语

                using (var welcomeReader = new StreamReader(new FileStream(Path.GetFullPath("config/welcome", AppContext.BaseDirectory), FileMode.Open, FileAccess.Read)))
                {
                    var random = new Random();
                    while (!welcomeReader.EndOfStream)
                    {
                        welcomeReader.ReadLine().ConsoleWrite((ConsoleColor)random.Next(1, 15));
                    }
                }

                #endregion

                "程序启动中.".ConsoleWrite();
                "正在读取配置.".ConsoleWrite();

                var config = new ConfigHelper(ConfigPath).GetModel<Config>("Config");

                if (config == null)
                {
                    $"配置读取失败, {ConfigPath} Section: Config.".ConsoleWrite();
                    return 1;
                }

                $"版本: {config.Version}.\r\n".ConsoleWrite();

                if (DataSource.IsNullOrWhiteSpace())
                {
                    $"未设置数据源.".ConsoleWrite();
                    return 1;
                }

                config.DataSource = DataSource;
                $"数据源: {DataSource}.\r\n".ConsoleWrite();

                config.DataSourceType = DataSourceType;
                $"数据源类型: {DataSourceType}.\r\n".ConsoleWrite();

                if (OutputPath.IsNullOrWhiteSpace())
                {
                    $"未设置输出路径.".ConsoleWrite();
                    return 1;
                }

                config.OutputPath = OutputPath;
                $"输出路径: {OutputPath}.\r\n".ConsoleWrite();
                config.GenType = GenType;
                $"生成类型: {GenType}.\r\n".ConsoleWrite();
                config.OverlayFile = OverlayFile;
                $"{(OverlayFile ? "允许" : "禁止")}覆盖已有文件.\r\n".ConsoleWrite();
                config.LoggerType = LoggerType;
                $"日志组件类型: {LoggerType}.\r\n".ConsoleWrite();

                var services = new ServiceCollection();

                services.AddSingleton(config)
                    .AddLogging()
                    .RegisterNLog(config.LoggerType, config.MinLogLevel);

                if (config.DataSourceType == DataSourceType.DataBase)
                    services.RegisterFreeSql(config.DataSource, config.DataBaseType);

                var builder = new AutofacServiceProviderFactory().CreateBuilder(services);

                var handlerType = typeof(IHandler);
                var handlers = typeof(Program)
                    .GetTypeInfo()
                    .Assembly
                    .GetTypes()
                    .Where(o => handlerType.IsAssignableFrom(o) && o != handlerType)
                    .ToArray();
                builder.RegisterTypes(handlers);

                AutofacHelper.Container = builder.Build();

                "已应用Autofac容器.\r\n".ConsoleWrite();

                try
                {
                    await AutofacHelper.GetService<GenerateHandler>().Handler();
                }
                catch (Exception ex)
                {
                    Logger.Log(NLog.LogLevel.Error, LogType.系统异常, $"生成失败, {GetExceptionAllMsg(ex)}", null, ex);
                    return 1;
                }

                return 0;
            }
            catch
            {
                return 1;
            }
        }

        /// <summary>
        /// 获取异常消息
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string GetExceptionAllMsg(Exception ex)
        {
            var message = ex.Message;
            if (ex.InnerException != null)
                message += $" {GetExceptionAllMsg(ex.InnerException)}";
            return message;
        }
    }
}

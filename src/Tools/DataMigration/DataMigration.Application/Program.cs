using Autofac;
using Autofac.Extensions.DependencyInjection;
using DataMigration.Application.Configures;
using DataMigration.Application.Handler;
using DataMigration.Application.Log;
using DataMigration.Application.Model;
using FreeSql;
using FreeSql.Internal;
using McMaster.Extensions.CommandLineUtils;
using Microservice.Library.Configuration;
using Microservice.Library.ConsoleTool;
using Microservice.Library.Container;
using Microservice.Library.Extension;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DataMigration.Application
{
    [Command(Name = "DataMigration", Description = "数据迁移工具.")]
    [VersionOption("-v|--version", "v0.0.0.1-beta", Description = "版本信息.")]
    [HelpOption(Description = "帮助信息.")]
    class Program
    {
        static Task<int> Main(string[] args) => CommandLineApplication.ExecuteAsync<Program>(args);

        #region 参数

        //[Argument(1, Description = "参数1.")]
        //string Param1 { get; } = "";

        #endregion

        #region 配置

        [Option("-c|--ConfigPath", Description = "配置文件路径: 不指定时使用默认配置.")]
        public string ConfigPath { get; } = "jsonconfig/config.json";

        [Option("-l|--LoggerType", Description = "日志类型（默认Console）.")]
        public LoggerType LoggerType { get; } = LoggerType.Console;

        [Option("-sc|--SourceConnectingString", Description = "源数据库连接字符串.")]
        public string SourceConnectingString { get; }

        [Option("-st|--SourceDataType", Description = "源数据库类型.")]
        public DataType SourceDataType { get; }

        [Option("-tc|--TargetConnectingString", Description = "目标数据库连接字符串.")]
        public string TargetConnectingString { get; }

        [Option("-tt|--TargetDataType", Description = "目标数据库类型.")]
        public DataType TargetDataType { get; }

        [Option("-e|--EntityAssemblys", Description = "实体类命名空间（存在多个时使用半角逗号[,]分隔，未设置此值时，将会自动生成实体类）.")]
        string EntityAssemblys { get; } = null;

        [Option("-nc|--SyncStructureNameConvert", Description = "实体类名 -> 数据库表名&列名，命名转换规则（类名、属性名都生效）（默认None）.")]
        NameConvertType? SyncStructureNameConvert { get; } = null;

        [Option("-rt|--EntityRazorTemplateFile", Description = "实体类Razor模板文件.")]
        string EntityRazorTemplateFile { get; } = "RazorTemplates/实体类+特性+导航属性（支持子父级结构）.cshtml";

        [Option("-o|--OperationType", Description = "操作类型（默认All）.")]
        public OperationType OperationType { get; } = OperationType.All;

        #endregion

#pragma warning disable CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
#pragma warning disable IDE0051 // 删除未使用的私有成员
#pragma warning disable IDE0060 // 删除未使用的参数
        async Task<int> OnExecuteAsync(CommandLineApplication app, CancellationToken cancellationToken = default)
#pragma warning restore IDE0060 // 删除未使用的参数
#pragma warning restore IDE0051 // 删除未使用的私有成员
#pragma warning restore CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        {
            try
            {
                //if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                //    Console.SetBufferSize(150, Console.BufferHeight);

                #region 欢迎语

                "@@@@@@@@@@@@                       @".ConsoleWrite(ConsoleColor.Magenta);
                "@@@        @@@                   ,@@".ConsoleWrite(ConsoleColor.Green);
                "@@@         f@@     1@@@@@@    @@@@@@@@    ;@@@@@@".ConsoleWrite(ConsoleColor.Yellow);
                "@@@          @@f   Cf     @@;    ,@@      1C     @@L".ConsoleWrite(ConsoleColor.Cyan);
                "@@@          @@C          1@@    ,@@             .@@".ConsoleWrite(ConsoleColor.Cyan);
                "@@@          @@.    @@@@@@@@@    ,@@       @@@@@@@@@".ConsoleWrite(ConsoleColor.Cyan);
                "@@@         @@C   @@0     i@@    ,@@     G@@     .@@".ConsoleWrite(ConsoleColor.Yellow);
                "@@@       @@@     @@i     @@@    .@@     @@G     @@@".ConsoleWrite(ConsoleColor.Green);
                "@@@@@@@@@@f        @@@@@@0;@@     @@@@@   @@@@@@@.@@".ConsoleWrite(ConsoleColor.Magenta, null, true, 3);

                "\t\t                        @@                                                     @@L".ConsoleWrite(ConsoleColor.Magenta);
                "\t\t@@@@           0@@@     @@                                             .@      L@".ConsoleWrite(ConsoleColor.Green);
                "\t\t@@@@G         f@G@@                                                   ;@@".ConsoleWrite(ConsoleColor.Green);
                "\t\t@@ @@;       :@@ @@     @@       @@@@@. @@    1@@ i@@@   t@@@@@@    @@@@@@@@   @@;      1@@@@@@      .@@  @@@@@".ConsoleWrite(ConsoleColor.Yellow);
                "\t\t@@  @@       @@  @@     @@     @@0    C@@@    1@@@f     @t     @@,    ;@@      @@;    @@@     @@@    .@@@i   .@@L".ConsoleWrite(ConsoleColor.Yellow);
                "\t\t@@   @@     @@   @@     @@    @@,       @@    1@@              t@@    ;@@      @@;   @@8       ;@@   .@@      ,@@".ConsoleWrite(ConsoleColor.Cyan);
                "\t\t@@    @@   @@    @@     @@   ,@@        @@    1@@        @@@@@@@@@    ;@@      @@;   @@         @@   .@@       @@".ConsoleWrite(ConsoleColor.Cyan);
                "\t\t@@    ;@@ @@     @@     @@   .@@        @@    1@@      @@8     t@@    ;@@      @@;   @@:        @@   .@@       @@".ConsoleWrite(ConsoleColor.Cyan);
                "\t\t@@     L@@@:     @@     @@    @@@      @@@    1@@      @@:     @@@    :@@      @@;   .@@.      @@;   .@@       @@".ConsoleWrite(ConsoleColor.Yellow);
                "\t\t@@      0@L      @@     @@     1@@@@@@@ @@    1@@       @@@@@@C1@@     @@@@@   @@;     @@@@@@@@@     .@@       @@".ConsoleWrite(ConsoleColor.Yellow);
                "\t\t                                        @@".ConsoleWrite(ConsoleColor.Green);
                "\t\t                                       @@,".ConsoleWrite(ConsoleColor.Green);
                "\t\t                              :@@@@@@@@@".ConsoleWrite(ConsoleColor.Magenta, null, true, 7);

                "\t\t\t]]]]]]]]]]]`      ]]]]            ,]]]`\t\t,]]`                  ]/@@@@@@]`      ]]]]]]]]]]]]]]]]]]   ]]]]]]]]]]]]]`".ConsoleWrite(ConsoleColor.Cyan);
                "\t\t\t@@@@@@@@@@@@@@`    \\@@@`         =@@@^\t\t=@@^               ,@@@@@@[[@@@@@@`   @@@@@@@@@@@@@@@@@@   @@@@@@@@@@@@@@@@`".ConsoleWrite(ConsoleColor.Cyan);
                "\t\t\t@@@        ,@@@^    =@@@^       /@@@`  \t\t=@@^              @@@@`        ,@@@^         =@@^          @@@          ,@@@^".ConsoleWrite(ConsoleColor.Cyan);
                "\t\t\t@@@         =@@^      @@@\\     @@@/   \t\t=@@^             @@@/           ,@@@         =@@^          @@@           =@@^".ConsoleWrite(ConsoleColor.Cyan);
                "\t\t\t@@@        /@@@        \\@@@` ,@@@`    \t\t=@@^            =@@@                         =@@^          @@@           @@@^".ConsoleWrite(ConsoleColor.Cyan);
                "\t\t\t@@@@@@@@@@@@@^          ,@@@@@@@       \t\t=@@^            =@@^                         =@@^          @@@]]]]]]]/@@@@@`".ConsoleWrite(ConsoleColor.Cyan);
                "\t\t\t@@@      ,[\\@@@\\          \\@@@/     \t\t=@@^            =@@^                         =@@^          @@@@@@@@@@@@@@".ConsoleWrite(ConsoleColor.Cyan);
                "\t\t\t@@@          @@@^          @@@         \t\t=@@^            =@@@                         =@@^          @@@        ,@@@^".ConsoleWrite(ConsoleColor.Cyan);
                "\t\t\t@@@          =@@^          @@@         \t\t=@@^             @@@\\           =@@@         =@@^          @@@          @@@^".ConsoleWrite(ConsoleColor.Cyan);
                "\t\t\t@@@         /@@@`          @@@         \t\t=@@^              @@@@`        ,@@@^         =@@^          @@@          ,@@@`".ConsoleWrite(ConsoleColor.Cyan);
                "\t\t\t@@@@@@@@@@@@@@@`           @@@         \t\t=@@@@@@@@@@@@@     ,@@@@@@]]@@@@@@`          =@@^          @@@           =@@@ ".ConsoleWrite(ConsoleColor.Cyan);
                "\t\t\t[[[[[[[[[[[[               [[[         \t\t,[[[[[[[[[[[[[        [\\@@@@@@/`             ,[[`          [[[            [[[`".ConsoleWrite(ConsoleColor.Cyan, null, true, 7);

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

                if (SourceConnectingString.IsNullOrWhiteSpace())
                {
                    $"未设置源数据库连接字符串.".ConsoleWrite();
                    return 1;
                }

                config.SourceConnectingString = SourceConnectingString;
                //$"源数据库: {SourceConnectingString}.\r\n".ConsoleWrite();

                config.SourceDataType = SourceDataType;
                $"源数据库类型: {SourceDataType}.\r\n".ConsoleWrite();

                if (TargetConnectingString.IsNullOrWhiteSpace())
                {
                    $"未设置目标数据库连接字符串.".ConsoleWrite();
                    return 1;
                }

                config.TargetConnectingString = TargetConnectingString;
                //$"目标数据库: {TargetConnectingString}.\r\n".ConsoleWrite();

                config.TargetDataType = TargetDataType;
                $"目标数据库类型: {TargetDataType}.\r\n".ConsoleWrite();

                if (EntityAssemblys.IsNullOrWhiteSpace())
                {
                    $"未设置实体类命名空间，将使用工具自动生成实体类.".ConsoleWrite();
                    config.EntityAssemblys = new List<string> { $"Entitys_{Guid.NewGuid():N}" };
                    config.GenerateEntitys = true;
                    $"实体类命名空间: {config.EntityAssemblys[0]}.\r\n".ConsoleWrite();
                }
                else
                {
                    config.EntityAssemblys = EntityAssemblys.Split(',').ToList();
                    $"实体类命名空间: {EntityAssemblys}.\r\n".ConsoleWrite();
                }

                config.OperationType = OperationType;
                $"操作类型: {OperationType}.\r\n".ConsoleWrite();
                config.SyncStructureNameConvert = SyncStructureNameConvert;
                $"实体类名 -> 数据库表名&列名，命名转换规则: {SyncStructureNameConvert}.\r\n".ConsoleWrite();
                config.EntityRazorTemplateFile = Path.IsPathRooted(EntityRazorTemplateFile) ? EntityRazorTemplateFile : Path.GetFullPath(EntityRazorTemplateFile, AppContext.BaseDirectory);
                $"实体类Razor模板文件: {EntityRazorTemplateFile}.\r\n".ConsoleWrite();

                config.LoggerType = LoggerType;
                $"日志组件类型: {LoggerType}.\r\n".ConsoleWrite();

                var services = new ServiceCollection();

                services.AddSingleton(config)
                    .AddLogging()
                    .RegisterNLog(config.LoggerType, config.MinLogLevel);

                services.RegisterFreeSql(config);

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
                    AutofacHelper.GetService<DataMigrationHandler>().Handler();
                }
                catch (Exception ex)
                {
                    Logger.Log(NLog.LogLevel.Error, LogType.系统异常, $"处理失败, {GetExceptionAllMsg(ex)}", null, ex);
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

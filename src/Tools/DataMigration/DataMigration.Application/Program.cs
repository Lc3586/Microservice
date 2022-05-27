using Autofac;
using Autofac.Extensions.DependencyInjection;
using DataMigration.Application.Configures;
using DataMigration.Application.Extension;
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
using Microservice.Library.FreeSql.Gen;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Logger = DataMigration.Application.Log.Logger;

namespace DataMigration.Application
{
    [Command(Name = "DataMigration", Description = "数据迁移工具.")]
    [VersionOption("-v|--version", "v0.0.0.1-beta", Description = "版本信息.")]
    [HelpOption(Description = "帮助信息.")]
    class Program
    {
        static Task<int> Main(string[] args)
        {
#if DEBUG
            if (args.Length == 0)
            {
                //调试时从系统环境变量中读取参数
                var val = Environment.GetEnvironmentVariable("DataMigration_Debug_Args", EnvironmentVariableTarget.User);
                val.ConsoleWrite(ConsoleColor.Cyan, "%DataMigration_Debug_Args%");

                args = Regex.Matches(val, "([^\" ][^ ]*)|(\"[^\"]*\")").Select(o => o.Value).ToArray();

                string.Join(" ", args).ConsoleWrite(ConsoleColor.Cyan, "args");
            }
#endif
            return CommandLineApplication.ExecuteAsync<Program>(args);
        }

        #region 参数

        //[Argument(1, Description = "参数1.")]
        //string Param1 { get; } = "";

        #endregion

        #region 配置

        [Option("-oc|--OutputChartset", Description = "控制台输出信息字符集.")]
        public string Chartset { get; }

        [Option("-c|--ConfigPath", Description = "配置文件路径: 不指定时使用默认配置.")]
        public string ConfigPath { get; } = "config/config.json";

        [Option("-l|--LoggerType", Description = "日志类型（默认Console）.")]
        public LoggerType LoggerType { get; } = LoggerType.Console;

        [Option("-mll|--MinLogLevel", Description = "需要记录的日志的最低等级（默认Info）.")]
        public string MinLogLevel { get; }

        [Option("-sc|--SourceConnectingString", Description = "源数据库连接字符串.")]
        public string SourceConnectingString { get; set; }

        [Option("-st|--SourceDataType", Description = "源数据库类型.")]
        public DataType SourceDataType { get; set; }

        [Option("-tc|--TargetConnectingString", Description = "目标数据库连接字符串.")]
        public string TargetConnectingString { get; set; }

        [Option("-tt|--TargetDataType", Description = "目标数据库类型.")]
        public DataType TargetDataType { get; set; }

        [Option("-e|--EntityAssembly", CommandOptionType.MultipleValue, Description = "实体类命名空间（存在多个时使用半角逗号[,]分隔，未设置此值时，将会自动生成实体类）.")]
        public List<string> EntityAssemblys { get; set; } = null;

        [Option("-nc|--SyncStructureNameConvert", Description = "实体类名 -> 数据库表名&列名，命名转换规则（类名、属性名都生效）（默认None）.")]
        public NameConvertType? SyncStructureNameConvert { get; } = null;

        [Option("-rt|--EntityRazorTemplateFile", Description = "实体类Razor模板文件.")]
        public string EntityRazorTemplateFile { get; } = "RazorTemplates/实体类+特性+导航属性（支持子父级结构）.cshtml";

        [Option("-o|--OperationType", Description = "操作类型（默认All）.")]
        public OperationType OperationType { get; set; } = OperationType.All;

        [Option("-dc|--DataCheck", Description = "数据检查（默认false）.")]
        public bool DataCheck { get; } = false;

        [Option("-dps|--DataPageSize", Description = "数据分页大小（默认10000）.")]
        public int DataPageSize { get; } = 10000;

        [Option("-dbc|--DisableBulkCopy", Description = "禁用批量插入功能（即使数据库支持也不使用此功能）（默认false）.")]
        public bool DisableBulkCopy { get; } = false;

        [Option("-sql|--UseSql", CommandOptionType.MultipleValue, Description = "使用自定义SQL查询语句（格式：$[表名(不区分大小写)]{SQL查询语句}, 示例: --UseSql \"$[TableA]{select * from TableA where Enable=1}\"）.")]
        public List<string> UseSql { get; }

        [Option("-tm|--TableMatch", CommandOptionType.MultipleValue, Description = "表名正则表达式，只生成匹配的表，如：dbo\\.TB_.+.（此值可以只针对指定的操作类型，如： --TableMatch \"$[Schema]{dbo\\.TB_.+}\" --TableMatch \"$[Data]{dbo\\.TB_A_.+}\"）")]
        public List<string> TableMatch { get; set; }

        [Option("-t|--Table", CommandOptionType.MultipleValue, Description = "指定数据库表（此值可以只针对指定的操作类型，如： --Table \"$[Data]{dbo.TB_A}\" --Table \"$[Data]{dbo.TB_B}\" --Table \"$[Data]{dbo.TB_B}\"）.")]
        public List<string> Tables { get; set; }

        [Option("-et|--ExclusionTable", CommandOptionType.MultipleValue, Description = "排除数据库表（此值可以只针对指定的操作类型，如： --ExclusionTable \"$[Data]{dbo.TB_A}\" --ExclusionTable \"$[Data]{dbo.TB_B}\"）.")]
        public List<string> ExclusionTables { get; set; }

        #endregion

#pragma warning disable IDE0051 // 删除未使用的私有成员
        async Task<int> OnExecuteAsync(CommandLineApplication app, CancellationToken cancellationToken = default)
#pragma warning restore IDE0051 // 删除未使用的私有成员
        {
            try
            {
                if (!Chartset.IsNullOrWhiteSpace())
                {
                    //设置字符集
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    var encoding = Encoding.GetEncoding(Chartset);
                    Console.OutputEncoding = encoding;
                }

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

                config.LoggerType = LoggerType;
                $"日志组件类型: {LoggerType}.\r\n".ConsoleWrite();

                config.MinLogLevel = MinLogLevel.IsNullOrWhiteSpace() ?
#if DEBUG
                    LogLevel.Trace.Ordinal :
                    //LogLevel.Info.Ordinal :
#else
                    LogLevel.Info.Ordinal :
#endif
                    LogLevel.FromString(MinLogLevel).Ordinal;
                $"日志等级: {LogLevel.FromOrdinal(config.MinLogLevel).Name}.\r\n".ConsoleWrite();

                if (SourceConnectingString.IsNullOrWhiteSpace())
                {
                    $"未设置源数据库连接字符串.".ConsoleWrite();
                    return 1;
                }

                config.SourceConnectingString = SourceConnectingString;
#if DEBUG
                $"源数据库: {SourceConnectingString}.\r\n".ConsoleWrite();
#endif

                config.SourceDataType = SourceDataType;
                $"源数据库类型: {SourceDataType}.\r\n".ConsoleWrite();

                if (TargetConnectingString.IsNullOrWhiteSpace())
                {
                    $"未设置目标数据库连接字符串.".ConsoleWrite();
                    return 1;
                }

                config.TargetConnectingString = TargetConnectingString;
#if DEBUG
                $"目标数据库: {TargetConnectingString}.\r\n".ConsoleWrite();
#endif

                config.TargetDataType = TargetDataType;
                $"目标数据库类型: {TargetDataType}.\r\n".ConsoleWrite();

                if (config.SourceConnectingString.Equals(config.TargetConnectingString))
                {
                    config.SameDb = true;
                    "检测到源数据库和目标数据库为同一数据库.\r\n".ConsoleWrite();
                }

                if (!EntityAssemblys.Any_Ex())
                {
                    $"未设置实体类dll文件，将使用工具自动生成实体类.".ConsoleWrite();
                    config.EntityAssemblyFiles = new List<string> { EntityHandler.BuildFileAbsolutePath };
                    config.GenerateEntitys = true;
                    $"实体类dll文件: {config.EntityAssemblyFiles[0]}.\r\n".ConsoleWrite();
                }
                else
                {
                    config.EntityAssemblyFiles = EntityAssemblys.Select(o => Path.IsPathRooted(o) ? o : Path.GetFullPath(o, AppContext.BaseDirectory)).ToList();
                    $"实体类dll文件: {string.Join(";", EntityAssemblys)}.\r\n".ConsoleWrite();
                }

                config.OperationType = OperationType;
                $"操作类型: {OperationType}.\r\n".ConsoleWrite();
                config.DataCheck = DataCheck;
                $"数据检查: {(DataCheck ? '是' : '否')}.\r\n".ConsoleWrite();
                config.DataPageSize = DataPageSize;
                $"数据分页大小: {DataPageSize}.\r\n".ConsoleWrite();
                config.UseBulkCopy = !DisableBulkCopy;
                $"禁用批量插入功能: {(DisableBulkCopy ? '是' : '否')}.\r\n".ConsoleWrite();
                config.UseSql = UseSql?.Select(o => o.Match(@$"[$][[](.*?)[]]{{(.*?)}}"))
                    .Where(o => o != null)
                    .ToDictionary(k => k[0].ToLower(), v => v[1]);

                config.TableMatch = new Dictionary<OperationType, List<string>>();
                TableMatch?.Select(o => o.Match(@$"[$][[](.*?)[]]{{(.*?)}}") ?? new List<string> { o })
                    .ForEach(o =>
                    {
                        if (o.Count == 1)
                            config.TableMatch.AddOrAppend(OperationType.All, o[0]);
                        else
                            config.TableMatch.AddOrAppend(o[0].ToEnum<OperationType>(), o[1]);
                    });
                $"表名正则表达式: {string.Join(";", config.TableMatch.Select(o => $"{o.Key} => {o.Value}"))}.\r\n".ConsoleWrite();

                config.Tables = new Dictionary<OperationType, List<string>>();
                Tables?.Select(o => o.Match(@$"[$][[](.*?)[]]{{(.*?)}}") ?? new List<string> { o })
                    .ForEach(o =>
                    {
                        if (o.Count == 1)
                            config.Tables.AddOrAppend(OperationType.All, o[0]);
                        else
                            config.Tables.AddOrAppend(o[0].ToEnum<OperationType>(), o[1]);
                    });
                $"指定数据库表: {string.Join(";", config.Tables.Select(o => $"{o.Key} => {string.Join(",", o.Value)}"))}.\r\n".ConsoleWrite();

                config.ExclusionTables = new Dictionary<OperationType, List<string>>();
                ExclusionTables?.Select(o => o.Match(@$"[$][[](.*?)[]]{{(.*?)}}") ?? new List<string> { o })
                    .ForEach(o =>
                    {
                        if (o.Count == 1)
                            config.ExclusionTables.AddOrAppend(OperationType.All, o[0]);
                        else
                            config.ExclusionTables.AddOrAppend(o[0].ToEnum<OperationType>(), o[1]);
                    });
                $"排除数据库表: {string.Join(";", config.ExclusionTables.Select(o => $"{o.Key} => {string.Join(",", o.Value)}"))}.\r\n".ConsoleWrite();

                config.SyncStructureNameConvert = SyncStructureNameConvert;
                $"实体类名 -> 数据库表名&列名，命名转换规则: {SyncStructureNameConvert}.\r\n".ConsoleWrite();
                config.EntityRazorTemplateFile = Path.IsPathRooted(EntityRazorTemplateFile) ? EntityRazorTemplateFile : Path.GetFullPath(EntityRazorTemplateFile, AppContext.BaseDirectory);
                $"实体类Razor模板文件: {EntityRazorTemplateFile}.\r\n".ConsoleWrite();

                var services = new ServiceCollection();

                services.AddSingleton(config)
                    .AddLogging()
                    .RegisterNLog(config.LoggerType, config.MinLogLevel)
                    .RegisterFreeSql(config);

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
                    await AutofacHelper.GetService<DataMigrationHandler>().Handler();
                }
                catch (Exception ex)
                {
                    Logger.Log(LogLevel.Error, LogType.系统异常, $"处理失败, {GetExceptionAllMsg(ex)}", null, ex);
                    return 1;
                }
                finally
                {
                    AutofacHelper.GetService<IFreeSqlMultipleProvider<int>>()?.Dispose();
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

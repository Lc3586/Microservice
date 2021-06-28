using Autofac.Extensions.DependencyInjection;
using McMaster.Extensions.CommandLineUtils;
using Microservice.Library.Configuration;
using Microservice.Library.ConsoleTool;
using Microservice.Library.Container;
using Microservice.Library.Extension;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Runtime.InteropServices;
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

        #region 配置

        [Option("-c|--ConfigPath", Description = "配置文件路径: 不指定时使用默认配置.")]
        public string ConfigPath { get; } = "jsonconfig/generateconfig.json";

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

        #endregion

        #region 参数

        [Argument(1, Description = "覆盖已有文件（默认不覆盖）.")]
        bool OverlayFile { get; } = false;

        #endregion

        async Task<int> OnExecuteAsync(CommandLineApplication app, CancellationToken cancellationToken = default)
        {
            try
            {
                //if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                //    Console.SetBufferSize(150, Console.BufferHeight);

                #region 欢迎语

                ",]]]]]]]]]]]]]]]]]`       ,@@^           ]/@@@@@@]`             ,]]]`               ,/@@@@@@\\]              ]/@@@@@@]`".ConsoleWrite(ConsoleColor.Magenta);
                "  =@@@@@@@@@@@@@^        /@@@^        ,@@@@@@[[@@@@@@`          @@@@@            ,@@@@@@[[@@@@@@\\        ,@@@@@@[[@@@@@@`".ConsoleWrite(ConsoleColor.Green);
                "      @@@              *@@@@@^       @@@@`        ,@@@^        /@@[@@@          /@@@`         \\@@@      @@@@`        ,@@@^".ConsoleWrite(ConsoleColor.Green);
                "      @@@             ,@@`=@@^      @@@/           ,@@@       =@@^ =@@\\        @@@/            \\@@^    @@@/           ,@@@".ConsoleWrite(ConsoleColor.Yellow);
                "      @@@            /@@  =@@^     =@@@                      =@@@   \\@@^      =@@@                    =@@@".ConsoleWrite(ConsoleColor.Yellow);
                "      @@@          *@@/   =@@^     =@@^                      @@@`    @@@`     =@@^                    =@@^".ConsoleWrite(ConsoleColor.Cyan);
                "      @@@         ,@@`    =@@^     =@@^                     @@@^     ,@@@     =@@^        =@@@@@@@@   =@@^".ConsoleWrite(ConsoleColor.Cyan);
                "      @@@        =@@]]]]]]/@@\\]]`  =@@@                    =@@@@@@@@@@@@@@    =@@@              @@@   =@@@".ConsoleWrite(ConsoleColor.Yellow);
                "      @@@        =@@@@@@@@@@@@@@^   @@@\\           =@@@   =@@@         \\@@\\    @@@\\             @@@    @@@\\           =@@@".ConsoleWrite(ConsoleColor.Yellow);
                "      @@@                 =@@^       @@@@`        ,@@@^  ,@@@`          @@@^    \\@@@`         ,@@@@     @@@@`        ,@@@^".ConsoleWrite(ConsoleColor.Green);
                "      @@@                 =@@^        ,@@@@@@]]@@@@@@`   @@@^           ,@@@^    ,@@@@@@]]@@@@@@@@@      ,@@@@@@]]@@@@@@`".ConsoleWrite(ConsoleColor.Green);
                "      [[[                 ,[[`           [\\@@@@@@/`     ,[[[             ,[[[       ,\\@@@@@@/`  ,[[         [\\@@@@@@/`\r\n\r\n\r\n\r\n\r\n\r\n\r\n".ConsoleWrite(ConsoleColor.Magenta);

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
                "\t\t\t[[[[[[[[[[[[               [[[         \t\t,[[[[[[[[[[[[[        [\\@@@@@@/`             ,[[`          [[[            [[[`\r\n\r\n\r\n\r\n\r\n\r\n\r\n".ConsoleWrite(ConsoleColor.Cyan);

                #endregion

                "程序启动中.".ConsoleWrite();
                "正在读取配置.".ConsoleWrite();

                var config = new ConfigHelper(ConfigPath)
                    .GetModel<GenerateConfig>("Config");

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
                $"日志组件类型: {GenType}.\r\n".ConsoleWrite();

                var services = new ServiceCollection();

                services.AddSingleton(config)
                    .AddLogging()
                    .RegisterNLog(config.LoggerType, config.MinLogLevel);

                if (config.DataSourceType == DataSourceType.DataBase)
                    services.RegisterFreeSql(config.DataSource, config.DataBaseType);

                AutofacHelper.Container = new AutofacServiceProviderFactory()
                    .CreateBuilder(services)
                    .Build();

                "已应用Autofac容器.\r\n".ConsoleWrite();

                try
                {
                    GenerateHandler.Generate();
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

        ///// <summary>
        ///// 获取命令参数
        ///// </summary>
        ///// <param name="input">输入</param>
        ///// <param name="modular">输出：模块</param>
        ///// <param name="method">输出：方法</param>
        ///// <param name="args">输出：参数</param>
        //private static void GetParam(string input, out string modular, out string method, out List<Arg_internal> args)
        //{
        //    modular = input.Substring(0, input.IndexOfN2L(" "));
        //    input = input.Substring(modular.Length).TrimStart(' ');
        //    if (input == "-h" || input == "--help")
        //    {
        //        method = input;
        //        args = null;
        //        return;
        //    }
        //    method = input.IndexOf('-') == 0 ? string.Empty : input.Substring(0, input.IndexOfN2L(" "));
        //    var _args = input.Length > 0 ? (method == string.Empty ? input : input.Substring(method.Length)) : string.Empty;
        //    args = Regex.Matches(_args + ' ', @"-(.*?)[\s\:\=](.*?)\s").Where(o => o.Groups != null && o.Groups.Count >= 2).Select(o => new Arg_internal() { Name = o.Groups[1].Value, Value = o.Groups[2].Value }).ToList();
        //}

        ///// <summary>
        ///// 处理命令
        ///// </summary>
        ///// <param name="modular">模块</param>
        ///// <param name="method">方法</param>
        ///// <param name="args">参数</param>
        ///// <returns></returns>
        //public static ProgramState Handler(string modular, string method, List<Arg_internal> args)
        //{
        //    try
        //    {

        //        switch (modular)
        //        {
        //            case "exit":
        //                "Exit!".ConsoleWrite(ConsoleColor.White, "info");
        //                return ProgramState.Exit;
        //            case "-h":
        //            case "--help":
        //                _CommandConfig.Description.ConsoleWrite(ConsoleColor.Cyan, "help");
        //                goto end;
        //        }

        //        var Modular = _CommandConfig.Modulars.FirstOrDefault(o => o.Name == modular);
        //        if (Modular == null)
        //            goto mismatch_modular;

        //        if (method == "-h" || method == "--help")
        //        {
        //            Modular.Description.ConsoleWrite(ConsoleColor.Cyan, "help");
        //            goto end;
        //        }

        //        var Method = Modular.Methods.FirstOrDefault(o => o.Name == method);
        //        if (Method == null)
        //            goto mismatch_method;

        //        var assembly = Modular.Path.IsNullOrEmpty() ? Assembly.GetExecutingAssembly() : Assembly.LoadFile($"{AppContext.BaseDirectory}\\{Modular.Path}");

        //        var obj = Method.Static ? null : assembly.CreateInstance(Method.TypeName, true, BindingFlags.Default, null, Method.IConfig ? new object[] { Configuration } : null, null, null);

        //        args = args.Where(o => o.Method_match.Contains(method)).ToList();
        //        var Params = new object[0];

        //        if (Method.Arg2Model != null)
        //        {
        //            var TargetAssembly = Method.Arg2Model.Path.IsNullOrEmpty() ? Assembly.GetExecutingAssembly() : Assembly.LoadFile($"{AppContext.BaseDirectory}\\{Method.Arg2Model.Path}");
        //            var TargetObj = TargetAssembly.CreateInstance(Method.Arg2Model.TypeName, true);

        //            foreach (var prop in TargetAssembly.GetType(Method.Arg2Model.TypeName).GetProperties())
        //            {
        //                var arg = args.Find(o => o.Name_Lower == prop.Name.ToLower());
        //                if (arg == null || arg.Value == null)
        //                    continue;
        //                prop.SetValue(TargetObj, arg.Value);
        //            }
        //            Params = new object[] { TargetObj };
        //        }
        //        else
        //        {
        //            Params = args.Select(o => o.Value).ToArray();
        //        }

        //        if (Method.Async)
        //            (assembly.GetType(Method.TypeName).GetMethod(Method.SpecifiedName).Invoke(obj, Params) as Task).Wait();
        //        else
        //            assembly.GetType(Method.TypeName).GetMethod(Method.SpecifiedName).Invoke(obj, Params);
        //        goto end;

        //        mismatch_modular:
        //        "无效指令!\t输入 -h|--help 获取帮助信息".ConsoleWrite(ConsoleColor.Yellow, "warn");
        //        goto end;
        //        mismatch_method:
        //        $"无效指令!\t输入 {modular} -h|--help 获取帮助信息".ConsoleWrite(ConsoleColor.Yellow, "warn");

        //        end:
        //        return ProgramState.Standby;
        //    }
        //    catch (Exception ex)
        //    {
        //        "T4 Code Automatic Generation Console Handler Error!".ConsoleWrite(ConsoleColor.Red, "error");
        //        ex.ConsoleWrite(ConsoleColor.Red, "error");
        //        return ProgramState.Error;
        //    }
        //}
    }
}

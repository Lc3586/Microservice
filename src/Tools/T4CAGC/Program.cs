using Autofac.Extensions.DependencyInjection;
using McMaster.Extensions.CommandLineUtils;
using Microservice.Library.Configuration;
using Microservice.Library.ConsoleTool;
using Microservice.Library.Container;
using Microservice.Library.Extension;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using T4CAGC.Config;
using T4CAGC.Handler;
using T4CAGC.Log;
using T4CAGC.Model;

namespace T4CAGC
{
    [Command(Name = "generate", Description = "代码自动生成工具.")]
    [VersionOption("v0.0.0.1-beta", Description = "当前为开发版本.")]
    [HelpOption]
    class Program
    {
        static Task<int> Main(string[] args) => CommandLineApplication.ExecuteAsync<Program>(args);
        //{


        //    var app = new CommandLineApplication
        //    {
        //        Description = config.Description
        //    };
        //    var version = app.VersionOption("--version", config.Version);
        //    version.Description = config.Description;
        //    var help = app.HelpOption();
        //    return app.ExecuteAsync(args);

        //    config.Command.Modulars
        //        .OrderBy(modular => modular.Sort)
        //        .ForEach(modular =>
        //        {
        //            app.Command(modular.Name, command =>
        //            {
        //                command.HelpOption("-h|--help");
        //                command.Description = modular.Description;

        //                var argument = command.Argument("method", "指定要执行的方法");

        //                List<CommandOption> options = new List<CommandOption>();
        //                Dictionary<string, Arg> options_arg = new Dictionary<string, Arg>();
        //                modular.Args
        //                    .OrderBy(o => o.Sort)
        //                    .ForEach(arg =>
        //                    {
        //                        var option = command.Option(arg.Name, arg.Description, arg.Type_enum);
        //                        options.Add(option);
        //                        options_arg.Add(option.LongName, arg);
        //                    });
        //                command.OnExecute(() =>
        //                    Handler(command.Name, argument.Value, options.Select(o => new Arg_internal()
        //                    {
        //                        Name = o.LongName,
        //                        Method = options_arg[o.LongName].Method,
        //                        Value = o.OptionType == CommandOptionType.MultipleValue
        //                        ? o.Values == null || o.Values.Count == 0 ? (options_arg[o.LongName].Default.IsNullOrEmpty() ? null : (options_arg[o.LongName].DataType == "System.String" ? options_arg[o.LongName].Default.ToString().Split("&&").ToList() : (object)options_arg[o.LongName].Default.ToString().Split("&&").Select(v => v.ConvertToAny<object>(null, options_arg[o.LongName].DataType_Type)).ToList())) : (options_arg[o.LongName].DataType == "System.String" ? o.Values : (object)o.Values?.Select(v => v.ConvertToAny<object>(null, options_arg[o.LongName].DataType_Type)).ToList())
        //                        : o.Value().ConvertToAny(Convert.ChangeType(options_arg[o.LongName].Default, options_arg[o.LongName].DataType_Type), options_arg[o.LongName].DataType_Type)
        //                    }).ToList())
        //                );
        //            });
        //        });

        //}

        #region 配置

        [Option("-C|--ConfigPath", Description = "配置文件路径: 不指定时使用默认配置.")]
        public string ConfigPath { get; } = null;

        [Option("-p|--OutputPath", Description = "输出路径. 默认程序根目录下 ./output文件夹")]
        public string OutputPath { get; } = "./output";

        [Option("-ds|--DataSource", Description = "数据源: CSV文件路径、数据库连接字符串.")]
        string DataSource { get; }

        [Option("-dt|--DataSourceType", Description = "数据源类型: CSV (电子表格)、DataBase(数据库). 默认为CSV.")]
        public DataSourceType DataSourceType { get; } = DataSourceType.CSV;

        [Option("-g|--GenType", Description = "生成类型: All (全部)、Api(接口项目)、Business(业务类库)、Model(业务模型类库)、Entity(实体类库)、Single(单个项目). 默认为All.")]
        public GenType GenType { get; } = GenType.All;

        [Option("-st|--SpecifyTable", Description = "指定表: 多张表请使用英文逗号进行分隔[,]. 默认指定所有的表.")]
        public string SpecifyTable { get; } = null;

        [Option("-it|--IgnoreTable", Description = "忽略表: 多张表请使用英文逗号进行分隔[,].")]
        public string IgnoreTable { get; } = null;

        [Option("-lt|--LoggerType", Description = "日志组件类型: Console(输出到控制台)、File(使用txt文件记录日志)、RDBMS(使用关系型数据库记录日志)、ElasticSearch(使用ElasticSearch记录日志). 默认 Console.")]
        public LoggerType LoggerType { get; } = LoggerType.Console;

        [Option("-ll|--MinLogLevel", Description = "需要记录的日志的最低等级: 0(Trace)、1(Debug)、2(Info)、3(Warn)、4(Error)、5(Fatal)、6(Off). 默认 0(Trace).")]
        public int MinLogLevel { get; } = 0;

        /// <summary>
        /// 指定的表
        /// </summary>
        List<string> SpecifyTableList => SpecifyTable?.Split(',').ToList();

        /// <summary>
        /// 忽略的表
        /// </summary>
        List<string> IgnoreTableList => IgnoreTable?.Split(',').ToList();

        #endregion

        #region 参数

        //[Argument(1, Description = "数据源: CSV文件路径、数据库连接字符串.")]
        //string DataSource { get; }

        #endregion

        async Task<int> OnExecuteAsync(CommandLineApplication app, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(OutputPath))
            {
                app.ShowHelp();
                return 0;
            }

            try
            {
                "程序启动中.".ConsoleWrite();
                "正在读取配置.".ConsoleWrite();

                var config = new ConfigHelper(ConfigPath)
                    .GetModel<GenerateConfig>("Config");

                if (config == null)
                {
                    $"配置读取失败, {ConfigPath} Section: Config.".ConsoleWrite();
                    return 1;
                }

                var services = new ServiceCollection();

                services.AddSingleton(config)
                    .AddLogging()
                    .RegisterNLog(LoggerType, MinLogLevel);

                AutofacHelper.Container = new AutofacServiceProviderFactory()
                    .CreateBuilder(services)
                    .Build();

                "已应用Autofac容器.".ConsoleWrite();

                var tables = DataSourceType switch
                {
                    DataSourceType.CSV => CSVHandler.Analysis(DataSource),
                    _ => DataBaseHandler.Analysis(DataSource)
                };

                //if (GenType == GenType.All)
                //    GenerateHandler.GenerateAll(tables, config);
                //else if (GenType == GenType.Single)
                //    GenerateHandler.GenerateSingle(tables, config);
                //else
                //{
                //    if (GenType == GenType.Api)
                //        GenerateHandler.GenerateApi(tables, config);
                //    if (GenType == GenType.Business)
                //        GenerateHandler.GenerateBusiness(tables, config);
                //    if (GenType == GenType.Model)
                //        GenerateHandler.GenerateModel(tables, config);
                //    if (GenType == GenType.Entity)
                //        GenerateHandler.GenerateEntity(tables, config);
                //}

                return 0;
            }
            catch
            {
                return 1;
            }
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

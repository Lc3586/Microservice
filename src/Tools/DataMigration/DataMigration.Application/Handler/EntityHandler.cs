using DataMigration.Application.Log;
using DataMigration.Application.Model;
using Microservice.Library.Extension;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DataMigration.Application.Handler
{
    /// <summary>
    /// 实体类处理器
    /// </summary>
    public class EntityHandler : IHandler
    {
        public EntityHandler(Config config)
        {
            Config = config;
            if (!Directory.Exists(TempDirectoryAbsolutePath))
                Directory.CreateDirectory(TempDirectoryAbsolutePath);
        }

        /// <summary>
        /// 配置
        /// </summary>
        readonly Config Config;

        /// <summary>
        /// 临时文件存放目录
        /// </summary>
        static readonly string TempDirectory = "Entitys.Project";

        /// <summary>
        /// 临时文件存放路径
        /// </summary>
        static readonly string TempDirectoryAbsolutePath = Path.GetFullPath(TempDirectory, AppContext.BaseDirectory);

        /// <summary>
        /// 项目生成文件存放路径
        /// </summary>
        static readonly string BuildDirectoryAbsolutePath = Path.Combine(TempDirectoryAbsolutePath, "Build");

        /// <summary>
        /// 项目生成文件存放路径
        /// </summary>
        public static string BuildFileAbsolutePath(string entityAssembly) => Path.Combine(BuildDirectoryAbsolutePath, $"{entityAssembly}.dll");

        /// <summary>
        /// 处理
        /// </summary>
        public void Handler()
        {
            var flag = false;

            if (Config.GenerateEntitys)
            {
                Generate().GetAwaiter().GetResult();
                flag = Config.EntityAssemblys.Select(o => Assembly.LoadFile(o).GetTypes()).Any();
            }
            else
            {
                flag = Config.EntityAssemblys.Select(o => Assembly.Load(o).GetTypes()).Any();
            }

            if (!flag)
                throw new ApplicationException($"指定命名空间[{string.Join(',', Config.EntityAssemblys)}]下未找到任何的实体类.");
        }

        /// <summary>
        /// 生成
        /// </summary>
        async Task Generate()
        {
            await InstallTool();

            await CreateCSProject();

            await CallTool();

            await BuildCSProject();
        }

        /// <summary>
        /// 安装工具
        /// </summary>
        /// <returns></returns>
        async Task InstallTool()
        {
            if (await CheckTool())
                return;

            try
            {
                await CallCmd("dotnet tool install -g FreeSql.Generator");
            }
            catch (Exception ex)
            {
                throw new ApplicationException("FreeSql.Generator安装失败.", ex);
            }

            if (!await CheckTool())
                throw new ApplicationException("FreeSql.Generator安装失败.");
        }

        /// <summary>
        /// 检查工具是否可用
        /// </summary>
        /// <returns></returns>
        async Task<bool> CheckTool()
        {
            try
            {
                await CallCmd("FreeSql.Generator --help");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log(NLog.LogLevel.Error, LogType.系统异常, "未安装FreeSql.Generator.", null, ex);
                return false;
            }
        }

        /// <summary>
        /// 调用工具
        /// </summary>
        /// <returns></returns>
        async Task CallTool()
        {
            var cmd = $"FreeSql.Generate -Razor 2 -NameSpace \"DataMigration.Entitys\" -DB \"{Config.SourceDataType},{Config.SourceConnectingString}\" -Output \"{TempDirectoryAbsolutePath}\"";
            await CallCmd(cmd, AppContext.BaseDirectory);

        }

        /// <summary>
        /// 创建实体类项目
        /// </summary>
        /// <returns></returns>
        async Task CreateCSProject()
        {
            var cmd = $"dotnet new classlib --language \"C#\" --framework \"netstandard2.0\" -n \"{Config.EntityAssemblys[0]}\" -o \"{TempDirectoryAbsolutePath}\"";
            await CallCmd(cmd, AppContext.BaseDirectory);

            //安装Nuget包
            var packages = new string[] { "Newtonsoft.Json", "FreeSql" };
            foreach (var package in packages)
            {
                var cmd_nuget = $"dotnet add package \"{package}\"";
                await CallCmd(cmd_nuget, AppContext.BaseDirectory);
            }
        }

        /// <summary>
        /// 生成实体类项目
        /// </summary>
        /// <returns></returns>
        async Task BuildCSProject()
        {
            var configuration = "Release";
#if DEBUG
            configuration = "Debug";
#endif

            var cmd = $"dotnet build --configuration {configuration} -o \"{BuildDirectoryAbsolutePath}\"";
            await CallCmd(cmd, AppContext.BaseDirectory);
        }

        /// <summary>
        /// 调用命令
        /// </summary>
        /// <param name="arguments">命令参数</param>
        /// <param name="workingDirectory">工作目录</param>
        /// <returns></returns>
        async Task CallCmd(string arguments, string workingDirectory = null)
        {
            using var process = GetProcess(arguments, workingDirectory);
            process.Start();

            var output = await process.StandardOutput.ReadToEndAsync();
            var error = process.StandardError.ReadToEnd();

#if DEBUG
            Console.Write(output);
            Console.Write(error);
#endif

            process.WaitForExit();
        }

        /// <summary>
        /// 获取进程
        /// </summary>
        /// <param name="arguments">参数</param>
        /// <param name="workingDirectory">工作目录</param>
        /// <returns></returns>
        Process GetProcess(string arguments, string workingDirectory = null)
        {
            var process = new Process();
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            process.StartInfo.RedirectStandardOutput = true;
            //process.StartInfo.StandardOutputEncoding = new UTF8Encoding(true);
            process.StartInfo.RedirectStandardError = true;
            //process.StartInfo.StandardErrorEncoding = new UTF8Encoding(true);

            process.StartInfo.Arguments = arguments;
            if (!workingDirectory.IsNullOrWhiteSpace())
                process.StartInfo.WorkingDirectory = workingDirectory;

            return process;
        }
    }
}

using DataMigration.Application.Log;
using DataMigration.Application.Model;
using Microservice.Library.Extension;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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
            if (config.GenerateEntitys && !Directory.Exists(TempDirectoryAbsolutePath))
                Directory.CreateDirectory(TempDirectoryAbsolutePath);
        }

        /// <summary>
        /// 配置
        /// </summary>
        readonly Config Config;

        /// <summary>
        /// 临时项目名称
        /// </summary>
        public static readonly string TempProjectName = $"Entity_{DateTime.Now.ToFileTimeUtc()}";

        /// <summary>
        /// 临时文件存放目录
        /// </summary>
        static readonly string TempDirectory = $"Entitys.Project/{TempProjectName}";

        /// <summary>
        /// 临时文件存放路径
        /// </summary>
        static readonly string TempDirectoryAbsolutePath = Path.GetFullPath(TempDirectory, AppContext.BaseDirectory);

        /// <summary>
        /// 项目生成文件存放路径
        /// </summary>
        static readonly string BuildDirectoryAbsolutePath = Path.Combine(TempDirectoryAbsolutePath, "Build",
#if DEBUG
             "Debug"
#else
             "Release"
#endif
            );

        /// <summary>
        /// 项目生成文件存放路径
        /// </summary>
        public static string BuildFileAbsolutePath = Path.Combine(BuildDirectoryAbsolutePath, $"{TempProjectName}.dll");

        /// <summary>
        /// 脚本文件存放路径
        /// </summary>
        static readonly string ShellDirectoryAbsolutePath = Path.GetFullPath("shell", AppContext.BaseDirectory);

        /// <summary>
        /// 处理
        /// </summary>
        public void Handler()
        {
            if (Config.GenerateEntitys)
                Generate().GetAwaiter().GetResult();

            if (!Config.EntityAssemblyFiles.SelectMany(o => Assembly.LoadFile(o).GetTypes()).Any())
                throw new ApplicationException($"未找到指定实体类dll文件[{string.Join(',', Config.EntityAssemblyFiles)}].");

            var types = Config.EntityAssemblyFiles.SelectMany(o => Assembly.LoadFile(o).GetTypes()).ToList();
            types.ForEach(o => Logger.Log(NLog.LogLevel.Info, LogType.系统信息, $"已获取实体类: {o.FullName}."));
            Extension.Extension.SetEntityTypes(types);
        }

        /// <summary>
        /// 生成
        /// </summary>
        async Task Generate()
        {
            Logger.Log(NLog.LogLevel.Info, LogType.系统信息, "安装.NET SDK.");

            await InstallDotnetSDK();

            Logger.Log(NLog.LogLevel.Info, LogType.系统信息, "安装FreeSql.Generator.");

            await InstallFreeSqlTool();

            Logger.Log(NLog.LogLevel.Info, LogType.系统信息, "创建实体类项目.");

            await CreateCSProject();

            Logger.Log(NLog.LogLevel.Info, LogType.系统信息, "生成实体类.");

            await CallFreeSqlTool();

            Logger.Log(NLog.LogLevel.Info, LogType.系统信息, "生成项目.");

            await BuildCSProject();
        }

        /// <summary>
        /// 安装.NET SDK
        /// </summary>
        /// <returns></returns>
        async Task InstallDotnetSDK()
        {
            if (await CheckDotnetSDK())
                return;

            try
            {
                string cmd;
                if (Config.CurrentOS == OSPlatform.Windows)
                    cmd = "./dotnet-install.ps1 -Channel LTS";
                else if (Config.CurrentOS == OSPlatform.Linux || Config.CurrentOS == OSPlatform.OSX)
                    cmd = "./dotnet-install.sh --channel LTS";
                else
                    throw new ApplicationException("不支持在当前操作系统执行此操作.");

                await CallCmd(cmd, null, ShellDirectoryAbsolutePath);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(".NET SDK安装失败.", ex);
            }

            if (!await CheckDotnetSDK())
                throw new ApplicationException(".NET SDK安装失败.");
        }

        /// <summary>
        /// 检查.NET SDK
        /// </summary>
        /// <returns></returns>
        async Task<bool> CheckDotnetSDK()
        {
            try
            {
                var result = await CallCmd("dotnet --info");
                return result.Contains(".NET SDK");
            }
            catch (Exception ex)
            {
                Logger.Log(NLog.LogLevel.Warn, LogType.警告信息, "未安装.NET SDK.", null, ex);
                return false;
            }
        }

        /// <summary>
        /// 安装FreeSql工具
        /// </summary>
        /// <returns></returns>
        async Task InstallFreeSqlTool()
        {
            if (await CheckFreeSqlTool())
                return;

            try
            {
                await CallCmd("dotnet tool install -g FreeSql.Generator");
            }
            catch (Exception ex)
            {
                throw new ApplicationException("FreeSql.Generator安装失败.", ex);
            }

            if (!await CheckFreeSqlTool())
                throw new ApplicationException("FreeSql.Generator安装失败.");
        }

        /// <summary>
        /// 检查FreeSql工具是否可用
        /// </summary>
        /// <returns></returns>
        async Task<bool> CheckFreeSqlTool()
        {
            try
            {
                var result = await CallCmd("FreeSql.Generator --help");
                return result.Contains("FreeSql 快速生成数据库的实体类");
            }
            catch (Exception ex)
            {
                Logger.Log(NLog.LogLevel.Warn, LogType.警告信息, "未安装FreeSql.Generator.", null, ex);
                return false;
            }
        }

        /// <summary>
        /// 调用FreeSql工具
        /// </summary>
        /// <returns></returns>
        async Task CallFreeSqlTool()
        {
            try
            {
                var cmd = $"FreeSql.Generator -Razor \"{Config.EntityRazorTemplateFile}\" -NameSpace \"DataMigration.Entitys\" -DB \"{Config.SourceDataType},{Config.SourceConnectingString}\" -Output \"{TempDirectoryAbsolutePath}\"";
                if (Config.TableMatch?.ContainsKey(OperationType.All) == true)
                    cmd += $" -Match {Config.TableMatch[OperationType.All]}";
                await CallCmd(cmd, null, AppContext.BaseDirectory);
            }
            catch (Exception ex)
            {
                Logger.Log(NLog.LogLevel.Warn, LogType.警告信息, "生成实体类时发生异常.", null, ex);
            }
        }

        /// <summary>
        /// 创建实体类项目
        /// </summary>
        /// <returns></returns>
        async Task CreateCSProject()
        {
            var cmd = $"dotnet new classlib --language \"C#\" --framework \"netstandard2.0\" --force -n \"{TempProjectName}\" -o \"{TempDirectoryAbsolutePath}\"";
            await CallCmd(cmd, null, AppContext.BaseDirectory);

            //清除自动生成的cs文件
            foreach (var file in Directory.GetFiles(TempDirectoryAbsolutePath, "*.cs", SearchOption.TopDirectoryOnly))
            {
                File.Delete(file);
            }

            //安装Nuget包
            var packages = new string[] { "Newtonsoft.Json", "FreeSql" };
            foreach (var package in packages)
            {
                var cmd_nuget = $"dotnet add \"{TempProjectName}.csproj\" package \"{package}\"";
                await CallCmd(cmd_nuget, null, TempDirectoryAbsolutePath);
            }
        }

        /// <summary>
        /// 生成实体类项目
        /// </summary>
        /// <returns></returns>
        async Task BuildCSProject()
        {
            if (Config.Tables.Any_Ex() || Config.ExclusionTables.Any_Ex())
                //清除不需要的表
                foreach (var file in new DirectoryInfo(TempDirectoryAbsolutePath).GetFiles("*.cs", SearchOption.TopDirectoryOnly))
                {
                    if (!Config.Tables.Any_Ex(o => file.Name.IndexOf($"{o}.cs", StringComparison.OrdinalIgnoreCase) == 0))
                        file.Delete();

                    if (Config.ExclusionTables.Any_Ex(o => file.Name.IndexOf($"{o}.cs", StringComparison.OrdinalIgnoreCase) == 0))
                        file.Delete();
                }

            var configuration = "Release";
#if DEBUG
            configuration = "Debug";
#endif

            var cmd = $"dotnet build --configuration {configuration} -o \"{BuildDirectoryAbsolutePath}\"";
            await CallCmd(cmd, null, TempDirectoryAbsolutePath);
        }

        /// <summary>
        /// 调用命令
        /// </summary>
        /// <param name="cmd">命令</param>
        /// <param name="arguments">参数</param>
        /// <param name="workingDirectory">工作目录</param>
        /// <returns></returns>
        async Task<string> CallCmd(string cmd, string arguments = null, string workingDirectory = null)
        {
            using var process = GetCmdProcess(arguments, workingDirectory);
            process.Start();

            process.StandardInput.WriteLine($"{cmd.TrimEnd('&')}&exit");
            process.StandardInput.AutoFlush = true;

            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();

#if DEBUG
            Console.Write(output);
            Console.Write(error);
#endif

            process.WaitForExit();

            if (!error.IsNullOrWhiteSpace())
                throw new ApplicationException(error);

            return output;
        }

        /// <summary>
        /// 获取进程
        /// </summary>
        /// <param name="arguments">参数</param>
        /// <param name="workingDirectory">工作目录</param>
        /// <returns></returns>
        Process GetCmdProcess(string arguments, string workingDirectory = null)
        {
            var process = new Process();
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            process.StartInfo.RedirectStandardInput = true;
            //process.StartInfo.StandardInputEncoding = new UTF8Encoding(true);
            process.StartInfo.RedirectStandardOutput = true;
            //process.StartInfo.StandardOutputEncoding = new UTF8Encoding(true);
            process.StartInfo.RedirectStandardError = true;
            //process.StartInfo.StandardErrorEncoding = new UTF8Encoding(true);

            if (Config.CurrentOS == OSPlatform.Windows)
                process.StartInfo.FileName = "cmd.exe";
            else if (Config.CurrentOS == OSPlatform.Linux || Config.CurrentOS == OSPlatform.OSX)
                process.StartInfo.FileName = Environment.GetEnvironmentVariable("SHELL");
            else
                throw new ApplicationException("不支持在当前操作系统执行此操作.");

            process.StartInfo.Arguments = arguments;
            if (!workingDirectory.IsNullOrWhiteSpace())
                process.StartInfo.WorkingDirectory = workingDirectory;

            return process;
        }
    }
}

using AutoMapper;
using Business.Hub;
using Business.Implementation.Common;
using Business.Utils.Log;
using Microservice.Library.DataMapping.Gen;
using Microservice.Library.Extension;
using Microservice.Library.File;
using Microservice.Library.FreeSql.Gen;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Model.Utils.CAGC;
using Model.Utils.CAGC.CAGCDTO;
using Model.Utils.Log;
using Model.Utils.SignalR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Business.Utils.CAGC
{
    /// <summary>
    /// 自动生成代码业务类
    /// </summary>
    public class CAGCBusiness : BaseBusiness, ICAGCBusiness
    {
        #region DI

        public CAGCBusiness(
            IFreeSqlProvider freeSqlProvider,
            IAutoMapperProvider autoMapperProvider,
            IHttpContextAccessor httpContextAccessor,
            IHubContext<CAGCHub> cagcHub)
        {
            Orm = freeSqlProvider.GetFreeSql();
            Mapper = autoMapperProvider.GetMapper();
            HttpContextAccessor = httpContextAccessor;
            CAGCHub = cagcHub;

            TempDir = Path.Combine(Config.AbsoluteStorageDirectory, "CAGC", "Temp");
        }

        #endregion

        #region 私有成员

        readonly IFreeSql Orm;

        readonly IMapper Mapper;

        readonly IHttpContextAccessor HttpContextAccessor;

        readonly IHubContext<CAGCHub> CAGCHub;

        /// <summary>
        /// 缓存目录绝对路径
        /// </summary>
        readonly string TempDir;

        /// <summary>
        /// 获取进程
        /// </summary>
        /// <param name="arguments">参数</param>
        /// <returns></returns>
        Process GetProcess(string arguments)
        {
            var filePath = Config.GetFileAbsolutePath("CAGC");

            var process = new Process();
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            process.StartInfo.RedirectStandardOutput = true;
            //process.StartInfo.StandardOutputEncoding = new UTF8Encoding(true);
            process.StartInfo.RedirectStandardError = true;
            //process.StartInfo.StandardErrorEncoding = new UTF8Encoding(true);

            process.StartInfo.FileName = filePath;
            process.StartInfo.Arguments = arguments;

            return process;
        }

        /// <summary>
        /// 调用应用程序
        /// </summary>
        /// <param name="arguments">命令参数</param>
        /// <returns></returns>
        async Task CallEXE(string arguments)
        {
            using var process = GetProcess(arguments);
            process.Start();

            var index = 0;
            var lines = new string[10];
            while (!process.StandardOutput.EndOfStream)
            {
                lines[index] = process.StandardOutput.ReadLine();
#if DEBUG
                Console.WriteLine(lines[index]);
#endif
                index++;

                if (process.StandardOutput.EndOfStream || index == lines.Length)
                {
                    var content = string.Join("\r\n", lines, 0, index);
                    await SendSignalrInfo(content, CAGCHubMethod.Info);
                    index = 0;
                }
            }

            //var output = await process.StandardOutput.ReadToEndAsync();
            var error = process.StandardError.ReadToEnd();

#if DEBUG
            //Console.Write(output);
            Console.Write(error);
#endif

            process.WaitForExit();

            //await SendSignalrInfo(output, CAGCHubMethod.Info);
            await SendSignalrInfo(error, CAGCHubMethod.Error);
        }

        /// <summary>
        /// 调用应用程序并返回输出信息
        /// </summary>
        /// <param name="arguments">命令参数</param>
        /// <returns></returns>
        async Task<string> CallEXEAndGetOutput(string arguments)
        {
            using var process = GetProcess(arguments);
            process.Start();

            var output = await process.StandardOutput.ReadToEndAsync();
            var error = await process.StandardError.ReadToEndAsync();

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
        /// 发送信息
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="method">方法</param>
        /// <returns></returns>
        async Task SendSignalrInfo(string data, string method)
        {
            if (data.IsNullOrWhiteSpace())
                return;

            try
            {
                if (Hub.CAGCHub.Users.TryGetValue(Operator.AuthenticationInfo.Id, out string connectionId))
                    await CAGCHub.Clients.Client(connectionId).SendCoreAsync(
                        method,
                        new object[]
                        {
                            data
                        });
            }
            catch (Exception ex)
            {
                Logger.Log(
                    NLog.LogLevel.Error,
                    LogType.系统异常,
                    $"处理SignalR代码自动生成信息推送时异常.",
                    data,
                    ex);
            }
        }

        /// <summary>
        /// 遍历文件夹
        /// </summary>
        /// <param name="directory">文件夹</param>
        /// <param name="fileCount">文件数量</param>
        /// <param name="fileLength">文件总字节数</param>
        /// <param name="delete">删除文件夹</param>
        void ErgodicDirectorie(DirectoryInfo directory, out int fileCount, out long fileLength, bool delete = false)
        {
            fileCount = 0;
            fileLength = 0;

            if (!directory.Exists)
                return;

            var directories = directory.GetDirectories();
            foreach (var deep_directory in directories)
            {
                ErgodicDirectorie(deep_directory, out int deep_fileCount, out long deep_fileLength, delete);
                fileCount += deep_fileCount;
                fileLength += deep_fileLength;

                if (delete)
                    deep_directory.Delete();
            }

            var files = directory.GetFiles();

            foreach (var file in files)
            {
                fileCount++;
                fileLength += file.Length;

                if (delete)
                    file.Delete();
            }
        }

        #endregion

        #region 公共

        public async Task<string> GetVersionInfo()
        {
            var arguments = "-v";

            return await CallEXEAndGetOutput(arguments);
        }

        public Dictionary<string, string> GetGenTypes()
        {
            var nameAndDescriptionDic = new Dictionary<string, string>();

            var type = typeof(GenType);

            foreach (var item in Enum.GetValues<GenType>())
            {
                var attr = type.GetField(item.ToString()).GetCustomAttribute<DescriptionAttribute>();

                if (attr == null)
                    nameAndDescriptionDic.Add(item.ToString(), ((int)item).ToString());
                else
                    nameAndDescriptionDic.Add(attr.Description, item.ToString());
            }

            return nameAndDescriptionDic;
        }

        public async Task<string> GenerateByCSV(IFormFile file, string genType)
        {
            var path = Path.Combine(TempDir, DateTime.Now.ToString("yyyy-MM-dd"), genType, DateTime.Now.Ticks.ToString());
            var dataSourceFile = Path.Combine(path, file.FileName);
            var outputPath = Path.Combine(path, "output");

            var outputDir = new DirectoryInfo(outputPath);

            if (!outputDir.Exists)
                outputDir.Create();

            using var rs = file.OpenReadStream();
            rs.Seek(0, SeekOrigin.Begin);
            using (var fs = File.Create(dataSourceFile))
            {
                await rs.CopyToAsync(fs);
            }

            var arguments = "-c \"jsonconfig/generateconfig.json\" "
                         + $"-g \"{genType}\" "
                         + $"-s \"{dataSourceFile}\" "
                         + $"-p \"{outputPath}\" true";

            await CallEXE(arguments);

            if (!outputDir.GetDirectories().Any() && !outputDir.GetFiles().Any())
                throw new MessageException($"未生成任何文件.");

            var zipFile = Path.Combine(path, "output.zip");

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            ZipFile.CreateFromDirectory(outputPath, zipFile, CompressionLevel.Fastest, true, Encoding.GetEncoding("GB2312"));

            return Path.GetRelativePath(Path.Combine(Config.AbsoluteStorageDirectory, "CAGC", "Temp"), zipFile).Base64Encode();
        }

        public async Task Download(string key)
        {
            var zipFile = Path.GetFullPath(key.Base64Decode(), Path.Combine(Config.AbsoluteStorageDirectory, "CAGC", "Temp"));

            if (!zipFile.Exists())
                throw new MessageException("文件不存在或已被移除, 请尝试重新生成.");

            var response = HttpContextAccessor.HttpContext.Response;
            response.ContentType = "text/plain";
            response.Headers.Add("Content-Disposition", $"attachment; filename=\"{UrlEncoder.Default.Encode("生成文件.zip")}\"");

            await FileBusiness.ResponseFile(HttpContextAccessor.HttpContext.Request, HttpContextAccessor.HttpContext.Response, zipFile);
        }

        public TempInfo GetTempInfo()
        {
            ErgodicDirectorie(new DirectoryInfo(TempDir), out int count, out long length);

            return new TempInfo
            {
                OccupiedSpace = length.GetFileSize(),
                FileCount = count
            };
        }

        public ClearnTempResult ClearTemp()
        {
            ErgodicDirectorie(new DirectoryInfo(TempDir), out int count, out long length, true);

            return new ClearnTempResult
            {
                FreeSpace = length.GetFileSize(),
                FileCount = count
            };
        }

        #endregion
    }
}

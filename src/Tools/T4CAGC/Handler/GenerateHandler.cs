using Microservice.Library.Extension;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using T4CAGC.Log;
using T4CAGC.Model;
using T4CAGC.Template;

namespace T4CAGC.Handler
{
    /// <summary>
    /// 生成类
    /// </summary>
    public class GenerateHandler : IHandler
    {
        public GenerateHandler(
            Config config,
            DataSourceHandler dataSourceHandler)
        {
            Config = config;
            DataSourceHandler = dataSourceHandler;
            Encoding = GetEncoding();
        }

        /// <summary>
        /// 配置
        /// </summary>
        readonly Config Config;

        /// <summary>
        /// 数据源处理器
        /// </summary>
        readonly DataSourceHandler DataSourceHandler;

        /// <summary>
        /// 编码
        /// </summary>
        readonly Encoding Encoding;

        /// <summary>
        /// 处理
        /// </summary>
        /// <returns></returns>
        public async Task Handler()
        {
            await Task.Run(DataSourceHandler.Handler);

            Logger.Log(NLog.LogLevel.Info, LogType.系统信息, $"生成类型为: {Config.GenType}.");

            if (Config.GenType == GenType.CompleteProject)
                GenerateCompleteProject(Config.OutputPath);
            else if (Config.GenType == GenType.SmallProject)
                GenerateSmallProject(Config.OutputPath);

            foreach (var table in Extension.Extension.GetTableInfos())
            {
                Logger.Log(NLog.LogLevel.Info, LogType.系统信息, $"正在处理: {table.Remark} {table.Name}.");

                switch (Config.GenType)
                {
                    case GenType.CompleteProject:
                    case GenType.SmallProject:
                    case GenType.EnrichmentProject:
                        GenerateEnrichmentProject(table, Config.OutputPath);
                        break;
                    case GenType.Controller:
                        GenerateController(table, Config.OutputPath);
                        break;
                    case GenType.Implementation:
                        GenerateImplementation(table, Config.OutputPath);
                        break;
                    case GenType.Interface:
                        GenerateInterface(table, Config.OutputPath);
                        break;
                    case GenType.Business:
                        GenerateBusiness(table, Config.OutputPath);
                        break;
                    case GenType.DTO:
                        GenerateDTO(table, Config.OutputPath);
                        break;
                    case GenType.Const:
                        GenerateConst(table, Config.OutputPath);
                        break;
                    case GenType.Enum:
                        GenerateEnum(table, Config.OutputPath);
                        break;
                    case GenType.Model:
                        GenerateModel(table, Config.OutputPath);
                        break;
                    case GenType.Entity:
                        GenerateEntity(table, Config.OutputPath);
                        break;
                    default:
                        throw new ApplicationException($"不支持的生成类型 {Config.GenType}");
                }
            }

            Logger.Log(NLog.LogLevel.Info, LogType.系统信息, "生成结束.");
        }

        /// <summary>
        /// 获取编码
        /// </summary>
        /// <returns></returns>
        Encoding GetEncoding()
        {
            if (Config.EncodingName == Encoding.UTF8.EncodingName)
                return new UTF8Encoding(true);

            if (Config.EncodingName == Encoding.UTF32.EncodingName)
                return new UTF32Encoding(true, true);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            return Encoding.GetEncoding(Config.EncodingName);
        }

        /// <summary>
        /// 生成完整项目
        /// </summary>
        /// <param name="outputPath">输出路径</param>
        void GenerateCompleteProject(string outputPath)
        {
            var projectCodeZipFile = new FileInfo(Path.GetFullPath(Config.CompleteProjectCodeZipFile, AppContext.BaseDirectory));
            if (!projectCodeZipFile.Exists)
            {
                Logger.Log(NLog.LogLevel.Warn, LogType.警告信息, $"项目代码文件不存在: {projectCodeZipFile.FullName}.");

                if (Config.CompleteProjectCodeZipDownloadUri.IsNullOrWhiteSpace())
                    throw new ApplicationException("未设置项目代码下载地址.");

                Logger.Log(NLog.LogLevel.Info, LogType.系统信息, $"尝试从此地址下载项目代码文件: {Config.CompleteProjectCodeZipDownloadUri}.");

                using var client = new WebClient();
                client.DownloadFile(Config.CompleteProjectCodeZipDownloadUri, projectCodeZipFile.FullName);

                Logger.Log(NLog.LogLevel.Info, LogType.系统信息, $"项目代码文件下载完毕.");
            }

            Logger.Log(NLog.LogLevel.Info, LogType.系统信息, $"正在解压缩项目代码: {projectCodeZipFile.FullName}.");

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            ZipFile.ExtractToDirectory(projectCodeZipFile.FullName, outputPath, Encoding.GetEncoding("GB2312"), Config.OverlayFile);
        }

        /// <summary>
        /// 生成小型项目
        /// </summary>
        /// <param name="outputPath">输出路径</param>
#pragma warning disable CA1822 // 将成员标记为 static
#pragma warning disable IDE0060 // 删除未使用的参数
        void GenerateSmallProject(string outputPath)
#pragma warning restore IDE0060 // 删除未使用的参数
#pragma warning restore CA1822 // 将成员标记为 static
        {
            throw new ApplicationException("暂不支持生成小型项目.");
        }

        /// <summary>
        /// 填充项目
        /// </summary>
        /// <param name="table">表数据</param>
        /// <param name="outputPath">输出路径</param>
        void GenerateEnrichmentProject(TableInfo table, string outputPath)
        {
            var entityPath = Path.Combine(outputPath, "Entity");
            GenerateEntity(table, entityPath);

            var modelPath = Path.Combine(outputPath, "Model");
            GenerateModel(table, modelPath);

            var businessPath = Path.Combine(outputPath, "Business");
            GenerateBusiness(table, businessPath);

            var controllerPath = Path.Combine(outputPath, "Api", "Controllers");
            GenerateController(table, controllerPath);
        }

        /// <summary>
        /// 生成配置类
        /// </summary>
        /// <param name="table">表数据</param>
        /// <param name="outputPath">输出路径</param>
#pragma warning disable CA1822 // 将成员标记为 static
#pragma warning disable IDE0060 // 删除未使用的参数
#pragma warning disable IDE0051 // 删除未使用的私有成员
        void GenerateConfigures(TableInfo table, string outputPath)
#pragma warning restore IDE0051 // 删除未使用的私有成员
#pragma warning restore IDE0060 // 删除未使用的参数
#pragma warning restore CA1822 // 将成员标记为 static
        {
            throw new ApplicationException("暂不支持生成配置类.");
        }

        /// <summary>
        /// 生成控制器类
        /// </summary>
        /// <param name="table">表数据</param>
        /// <param name="outputPath">输出路径</param>
        void GenerateController(TableInfo table, string outputPath)
        {
            Logger.Log(NLog.LogLevel.Info, LogType.系统信息, $"正在生成控制器类: {table.Remark} {table.Name}.");

            var filename = Path.Combine(outputPath, table.ModuleName, $"{table.ReducedName}Controller.cs");
            var tt = new Controller(new ControllerOptions
            {
                Version = Config.Version,
                Table = table
            });

            if (tt.Ignore)
                return;

            OutputToFile(tt, filename);
        }

        /// <summary>
        /// 生成业务实现类
        /// </summary>
        /// <param name="table">表数据</param>
        /// <param name="outputPath">输出路径</param>
        void GenerateImplementation(TableInfo table, string outputPath)
        {
            Logger.Log(NLog.LogLevel.Info, LogType.系统信息, $"正在生成业务实现类: {table.Remark} {table.Name}.");

            var filename = Path.Combine(outputPath, table.ModuleName, $"{table.ReducedName}Business.cs");
            var tt = new Implementation(new ImplementationOptions
            {
                Version = Config.Version,
                Table = table
            });

            if (tt.Ignore)
                return;

            OutputToFile(tt, filename);
        }

        /// <summary>
        /// 生成业务接口类
        /// </summary>
        /// <param name="table">表数据</param>
        /// <param name="outputPath">输出路径</param>
        void GenerateInterface(TableInfo table, string outputPath)
        {
            Logger.Log(NLog.LogLevel.Info, LogType.系统信息, $"正在生成业务接口类: {table.Remark} {table.Name}.");

            var filename = Path.Combine(outputPath, table.ModuleName, $"I{table.ReducedName}Business.cs");
            var tt = new Interface(new InterfaceOptions
            {
                Version = Config.Version,
                Table = table
            });

            if (tt.Ignore)
                return;

            OutputToFile(tt, filename);
        }

        /// <summary>
        /// 生成业务实现类&业务接口类
        /// </summary>
        /// <param name="table">表数据</param>
        /// <param name="outputPath">输出路径</param>
        void GenerateBusiness(TableInfo table, string outputPath)
        {
            var interfacePath = Path.Combine(outputPath, "Interface");
            GenerateInterface(table, interfacePath);

            var implementationPath = Path.Combine(outputPath, "Implementation");
            GenerateImplementation(table, implementationPath);
        }

        /// <summary>
        /// 生成业务模型类
        /// </summary>
        /// <param name="table">表数据</param>
        /// <param name="outputPath">输出路径</param>
        void GenerateDTO(TableInfo table, string outputPath)
        {
            Logger.Log(NLog.LogLevel.Info, LogType.系统信息, $"正在生成业务模型类: {table.Remark} {table.Name}.");

            var filename = Path.Combine(outputPath, table.ModuleName, $"{table.ReducedName}DTO.cs");
            var tt = new DTO(new DTOOptions
            {
                Version = Config.Version,
                Table = table
            });

            if (tt.Ignore)
                return;

            OutputToFile(tt, filename);
        }

        /// <summary>
        /// 生成常量定义类
        /// </summary>
        /// <param name="table">表数据</param>
        /// <param name="outputPath">输出路径</param>
        void GenerateConst(TableInfo table, string outputPath)
        {
            Logger.Log(NLog.LogLevel.Info, LogType.系统信息, $"正在生成常量定义类: {table.Remark} {table.Name}.");

            if (table.RelationshipTable)
            {
                Logger.Log(NLog.LogLevel.Info, LogType.系统信息, $"表信息中存在联合主键, 且没有其他字段, 可能为关系表, 已跳过.", table.Name);
                return;
            }
            else if (!table.Fields.Any_Ex(o => o.Primary))
            {
                Logger.Log(NLog.LogLevel.Info, LogType.系统信息, $"表信息中未找到主键, 已跳过.", table.Name);
                return;
            }

            table.Fields.ForEach(o =>
            {
                if (!o.Consts.Any_Ex())
                    return;

                var filename = Path.Combine(outputPath, table.ModuleName, $"{table.ReducedName}_{o.Name}.cs");
                var tt = new Const(new ConstOptions
                {
                    Version = Config.Version,
                    TableRemark = table.Remark,
                    ReducedName = table.ReducedName,
                    ModuleName = table.ModuleName,
                    Field = o
                });

                OutputToFile(tt, filename);
            });
        }

        /// <summary>
        /// 生成枚举定义类
        /// </summary>
        /// <param name="table">表数据</param>
        /// <param name="outputPath">输出路径</param>
        void GenerateEnum(TableInfo table, string outputPath)
        {
            Logger.Log(NLog.LogLevel.Info, LogType.系统信息, $"正在生成枚举定义类: {table.Remark} {table.Name}.");

            if (table.RelationshipTable)
            {
                Logger.Log(NLog.LogLevel.Info, LogType.系统信息, $"表信息中存在联合主键, 且没有其他字段, 可能为关系表, 已跳过.", table.Name);
                return;
            }
            else if (!table.Fields.Any_Ex(o => o.Primary))
            {
                Logger.Log(NLog.LogLevel.Info, LogType.系统信息, $"表信息中未找到主键, 已跳过.", table.Name);
                return;
            }

            table.Fields.ForEach(o =>
            {
                if (!o.Enums.Any_Ex())
                    return;

                var filename = Path.Combine(outputPath, table.ModuleName, $"{table.ReducedName}_{o.Name}.cs");
                var tt = new Template.Enum(new EnumOptions
                {
                    Version = Config.Version,
                    TableRemark = table.Remark,
                    ReducedName = table.ReducedName,
                    ModuleName = table.ModuleName,
                    Field = o
                });

                OutputToFile(tt, filename);
            });
        }

        /// <summary>
        /// 生成业务模型类&常量定义类&枚举定义类
        /// </summary>
        /// <param name="table">表数据</param>
        /// <param name="outputPath">输出路径</param>
        void GenerateModel(TableInfo table, string outputPath)
        {
            GenerateDTO(table, outputPath);
            GenerateConst(table, outputPath);
            GenerateEnum(table, outputPath);
        }

        /// <summary>
        /// 生成实体模型类
        /// </summary>
        /// <param name="table">表数据</param>
        /// <param name="outputPath">输出路径</param>
        void GenerateEntity(TableInfo table, string outputPath)
        {
            Logger.Log(NLog.LogLevel.Info, LogType.系统信息, $"正在生成实体模型类: {table.Remark} {table.Name}.");

            var filename = Path.Combine(outputPath, table.ModuleName, $"{table.Name}.cs");
            var tt = new Entity(new EntityOptions
            {
                Version = Config.Version,
                Table = table
            });
            OutputToFile(tt, filename);
        }

        /// <summary>
        /// 输出至文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tt">模板对象</param>
        /// <param name="filename">文件路径以及文件名（包括拓展名）</param>
        void OutputToFile<T>(T tt, string filename)
        {
            var file = new FileInfo(filename);

            if (file.Exists && !Config.OverlayFile)
            {
                Logger.Log(NLog.LogLevel.Info, LogType.系统信息, $"文件已存在, 当前禁止覆盖文件, 已跳过.", filename);
                return;
            }

            if (!file.Directory.Exists)
                file.Directory.Create();

            var content = (string)tt.GetType().GetMethod("TransformText").Invoke(tt, null);

            //using var stream = file.Create();
            //stream.Write(Encoding.GetBytes(content));

            //此方法会添加编码签名信息
            File.WriteAllText(filename, content, Encoding);

            if (file.Exists)
                Logger.Log(NLog.LogLevel.Info, LogType.系统信息, $"重写文件.", filename);
            else
                Logger.Log(NLog.LogLevel.Info, LogType.系统信息, $"生成文件.", filename);
        }
    }
}

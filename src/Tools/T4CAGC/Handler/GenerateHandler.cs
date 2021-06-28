using Microservice.Library.Container;
using Microservice.Library.Extension;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using T4CAGC.Log;
using T4CAGC.Model;
using T4CAGC.Template;

namespace T4CAGC.Handler
{
    /// <summary>
    /// 生成类
    /// </summary>
    public static class GenerateHandler
    {
        /// <summary>
        /// 配置
        /// </summary>
        readonly static GenerateConfig Config = AutofacHelper.GetService<GenerateConfig>();

        /// <summary>
        /// 编码
        /// </summary>
        readonly static Encoding Encoding = GetEncoding();

        /// <summary>
        /// 表信息
        /// </summary>
        readonly static List<TableInfo> Tables = GetTables();

        /// <summary>
        /// 获取编码
        /// </summary>
        /// <returns></returns>
        static Encoding GetEncoding()
        {
            if (Config.EncodingName == Encoding.UTF8.EncodingName)
                return new UTF8Encoding(true);

            if (Config.EncodingName == Encoding.UTF32.EncodingName)
                return new UTF32Encoding(true, true);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            return Encoding.GetEncoding(Config.EncodingName);
        }

        /// <summary>
        /// 获取数据表信息
        /// </summary>
        /// <returns></returns>
        static List<TableInfo> GetTables()
        {
            var tables = Config.DataSourceType switch
            {
                DataSourceType.CSV => Config.DataSource.GetCSVData(Config.SpecifyTable, Config.IgnoreTable),
                DataSourceType.CSV_Simple => Config.DataSource.GetCSVData_Simple(Config.SpecifyTable, Config.IgnoreTable),
                _ => Config.TableType.GetDataBaseData(Config.SpecifyTable, Config.IgnoreTable)
            };

            return tables;
        }

        /// <summary>
        /// 生成完整项目
        /// </summary>
        /// <param name="table">表数据</param>
        /// <param name="outputPath">输出路径</param>
        static void GenerateCompleteProject(TableInfo table, string outputPath)
        {
            throw new ApplicationException("暂不支持生成完整项目.");
        }

        /// <summary>
        /// 生成小型项目
        /// </summary>
        /// <param name="table">表数据</param>
        /// <param name="outputPath">输出路径</param>
        static void GenerateSmallProject(TableInfo table, string outputPath)
        {
            throw new ApplicationException("暂不支持生成小型项目.");
        }

        /// <summary>
        /// 填充项目
        /// </summary>
        /// <param name="table">表数据</param>
        /// <param name="outputPath">输出路径</param>
        static void GenerateEnrichmentProject(TableInfo table, string outputPath)
        {
            var entityPath = Path.Combine(outputPath, "Entity");
            GenerateEntity(table, entityPath);

            var modelPath = Path.Combine(outputPath, "Model");
            GenerateModel(table, modelPath);

            var businessPath = Path.Combine(outputPath, "Business");
            GenerateBusiness(table, businessPath);

            var controllerPath = Path.Combine(outputPath, "Controller");
            GenerateController(table, controllerPath);
        }

        /// <summary>
        /// 生成配置类
        /// </summary>
        /// <param name="table">表数据</param>
        /// <param name="outputPath">输出路径</param>
        static void GenerateConfigures(TableInfo table, string outputPath)
        {

        }

        /// <summary>
        /// 生成控制器类
        /// </summary>
        /// <param name="table">表数据</param>
        /// <param name="outputPath">输出路径</param>
        static void GenerateController(TableInfo table, string outputPath)
        {
            Logger.Log(NLog.LogLevel.Info, LogType.系统跟踪, $"正在生成控制器类: {table.Remark} {table.Name}.");

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
        static void GenerateImplementation(TableInfo table, string outputPath)
        {
            Logger.Log(NLog.LogLevel.Info, LogType.系统跟踪, $"正在生成业务实现类: {table.Remark} {table.Name}.");

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
        static void GenerateInterface(TableInfo table, string outputPath)
        {
            Logger.Log(NLog.LogLevel.Info, LogType.系统跟踪, $"正在生成业务接口类: {table.Remark} {table.Name}.");

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
        static void GenerateBusiness(TableInfo table, string outputPath)
        {
            var implementationPath = Path.Combine(outputPath, "Implementation");
            GenerateImplementation(table, implementationPath);

            var interfacePath = Path.Combine(outputPath, "Interface");
            GenerateInterface(table, interfacePath);
        }

        /// <summary>
        /// 生成业务模型类
        /// </summary>
        /// <param name="table">表数据</param>
        /// <param name="outputPath">输出路径</param>
        static void GenerateDTO(TableInfo table, string outputPath)
        {
            Logger.Log(NLog.LogLevel.Info, LogType.系统跟踪, $"正在生成业务模型类: {table.Remark} {table.Name}.");

            var filename = Path.Combine(outputPath, table.ModuleName, $"{table.ReducedName}DTO.cs");
            var tt = new DTO(new DTOOptions
            {
                Version = Config.Version,
                Table = table
            });
            OutputToFile(tt, filename);
        }

        /// <summary>
        /// 生成常量定义类
        /// </summary>
        /// <param name="table">表数据</param>
        /// <param name="outputPath">输出路径</param>
        static void GenerateConst(TableInfo table, string outputPath)
        {
            Logger.Log(NLog.LogLevel.Info, LogType.系统跟踪, $"正在生成常量定义类: {table.Remark} {table.Name}.");

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
        static void GenerateEnum(TableInfo table, string outputPath)
        {
            Logger.Log(NLog.LogLevel.Info, LogType.系统跟踪, $"正在生成枚举定义类: {table.Remark} {table.Name}.");

            table.Fields.ForEach(o =>
            {
                if (!o.Enums.Any_Ex())
                    return;

                var filename = Path.Combine(outputPath, table.ModuleName, $"{table.ReducedName}_{o.Name}.cs");
                var tt = new T4CAGC.Template.Enum(new EnumOptions
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
        static void GenerateModel(TableInfo table, string outputPath)
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
        static void GenerateEntity(TableInfo table, string outputPath)
        {
            Logger.Log(NLog.LogLevel.Info, LogType.系统跟踪, $"正在生成实体模型类: {table.Remark} {table.Name}.");

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
        static void OutputToFile<T>(T tt, string filename)
        {
            var file = new FileInfo(filename);

            if (file.Exists && !Config.OverlayFile)
            {
                Logger.Log(NLog.LogLevel.Info, LogType.系统跟踪, $"文件已存在, 当前禁止覆盖文件, 已跳过.", filename);
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
                Logger.Log(NLog.LogLevel.Info, LogType.系统跟踪, $"重写文件.", filename);
            else
                Logger.Log(NLog.LogLevel.Info, LogType.系统跟踪, $"生成文件.", filename);
        }

        /// <summary>
        /// 生成
        /// </summary>
        /// <returns></returns>
        public static void Generate()
        {
            Logger.Log(NLog.LogLevel.Info, LogType.系统跟踪, $"共获取到了{Tables.Count}张表数据.");

            Logger.Log(NLog.LogLevel.Info, LogType.系统跟踪, $"生成类型为: {Config.GenType}.");

            foreach (var table in Tables)
            {
                Logger.Log(NLog.LogLevel.Info, LogType.系统跟踪, $"正在处理: {table.Remark} {table.Name}.");

                switch (Config.GenType)
                {
                    case GenType.CompleteProject:
                        GenerateCompleteProject(table, Config.OutputPath);
                        break;
                    case GenType.SmallProject:
                        GenerateSmallProject(table, Config.OutputPath);
                        break;
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

            Logger.Log(NLog.LogLevel.Info, LogType.系统跟踪, "生成结束.");
        }
    }
}

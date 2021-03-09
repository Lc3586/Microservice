using Microservice.Library.ConsoleTool;
using Microservice.Library.Container;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using T4CAGC.Model;

namespace T4CAGC.Handler
{
    /// <summary>
    /// 生成类
    /// </summary>
    public static class GenerateHandler
    {
        readonly static GenerateConfig Config = AutofacHelper.GetService<GenerateConfig>();

        readonly static List<TableInfo> Tables = GetTables();

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

        public static async Task Generate()
        {
            $"共获取到了{Tables.Count}张表信息.".ConsoleWrite();

            foreach (var table in Tables)
            {
                $"{table.Remark} {table.Name}.".ConsoleWrite();
            }
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
            //if (config.Language.IsNullOrEmpty())
            //    throw new Exception("Language must be certainty");
            //if (!config.DbType_Type.HasValue)
            //    throw new Exception("DbType is invalid");
            //if (!config.Database.Any_Ex())
            //    throw new Exception("Specify at least one Database");
            //if (!config.OutputPath.Any_Ex())
            //    config.OutputPath = new List<string>() { $"{AppContext.BaseDirectory}Output" };
            //foreach (var database in config.Database)
            //{
            //    var dbhelper = DbHelperFactory.GetDbHelper(config.DbType_Type.Value, $"{config.DbConnection};database={database};");

            //    var tables = dbhelper.GetDbTableInfo(database, config.Table, config.TableIgnore, true);

            //    foreach (var table in tables)
            //    {
            //        Model_Entity temple = new Model_Entity(config, table);
            //        string fileName = table.ClassName + ".cs";

            //        config.OutputPath.ForEach(op =>
            //        {
            //            if (!Directory.Exists(op))
            //                Directory.CreateDirectory(op);
            //            File.WriteAllText($"{op}\\{fileName}", temple.TransformText(), Encoding.UTF8);
            //        });
            //    }
            //}
        }
    }
}

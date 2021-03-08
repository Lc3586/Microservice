using Microservice.Library.Container;
using System.Collections.Generic;
using System.Linq;
using T4CAGC.Model;

namespace T4CAGC.Handler
{
    /// <summary>
    /// 生成类
    /// </summary>
    public class GenerateHandler
    {
        public GenerateHandler(List<TableInfo> tables)
        {
            Tables = tables;
        }

        readonly List<TableInfo> Tables;

        readonly GenerateConfig Config = AutofacHelper.GetService<GenerateConfig>();

        List<TableInfo> GetTables()
        {
            var tables = Config.DataSourceType switch
            {
                DataSourceType.CSV => DataSourceHandler.GetCSVData(
                    Config.DataSource,
                    Config.SpecifyTable?.Split(',').ToList(),
                    Config.IgnoreTable?.Split(',').ToList()),
                _ => DataSourceHandler.GetDataBaseData(
                    Config.TableType,
                    Config.SpecifyTable?.Split(',').ToList(),
                    Config.IgnoreTable?.Split(',').ToList())
            };

            return tables;
        }

        public static void Generate()
        {
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

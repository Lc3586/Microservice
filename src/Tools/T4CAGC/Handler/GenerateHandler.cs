using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using T4CAGC.Model;
using T4CAGC.Template;

namespace T4CAGC.Handler
{
    /// <summary>
    /// 生成类
    /// </summary>
    public class GenerateHandler
    {
        public GenerateHandler()
        {

        }

        public static void GenerateEntityModel(List<TableInfo> tables)
        {
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

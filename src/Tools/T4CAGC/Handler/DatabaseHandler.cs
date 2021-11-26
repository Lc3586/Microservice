using FreeSql.DatabaseModel;
using Microservice.Library.Extension;
using Microservice.Library.FreeSql.Gen;
using System;
using System.Collections.Generic;
using System.Linq;
using T4CAGC.Extension;
using T4CAGC.Model;

namespace T4CAGC.Handler
{
    /// <summary>
    /// 数据库处理类
    /// </summary>
    public class DatabaseHandler : IHandler
    {
        public DatabaseHandler(
            Config config,
            IFreeSqlProvider freeSqlProvider)
        {
            Config = config;
            IFreeSqlProvider = freeSqlProvider;
        }

        /// <summary>
        /// 配置
        /// </summary>
        readonly Config Config;

        /// <summary>
        /// 
        /// </summary>
        readonly IFreeSqlProvider IFreeSqlProvider;

        /// <summary>
        /// 处理
        /// </summary>
        /// <returns>数据表信息</returns>
        public List<TableInfo> Handler()
        {
            return AnalysisDatabase();
        }

        /// <summary>
        /// 解析CSV文件
        /// </summary>
        /// <returns></returns>
        public List<TableInfo> AnalysisDatabase()
        {
            var orm = IFreeSqlProvider.GetFreeSql();

            var tables = new List<DbTableInfo>();

            if (Config.SpecifyTables.Any_Ex())
                Config.SpecifyTables.ForEach(o => tables.Add(orm.DbFirst.GetTableByName(o, true)));
            else
                tables = orm.DbFirst.GetTablesByDatabase();

            if (Config.TableType.Any_Ex() || Config.IgnoreTables.Any_Ex())
                tables.RemoveAll(o =>
                (Config.TableType.Any_Ex() && !Config.TableType.Contains(o.Type))
                || (Config.IgnoreTables.Any_Ex() && Config.IgnoreTables.Any(p => string.Equals(o.Name, p, StringComparison.OrdinalIgnoreCase))));

            return tables.Select(dbTable =>
            {
                var tableInfo = new TableInfo
                {
                    Name = dbTable.Name,
                    Remark = dbTable.Comment,
                    Fields = dbTable.Columns
                        .OrderBy(column => column.Position)
                        .Select(column =>
                        {
                            var fieldInfo = new FieldInfo
                            {
                                Name = column.Name,
                                Remark = column.Coment,
                                DbType = column.DbTypeText,
                                Type = column.CsType.Name,
                                CsType = column.CsType,
                                Primary = column.IsPrimary,
                                Nullable = column.IsNullable,
                                Length = column.MaxLength
                            };

                            return fieldInfo;
                        })
                        .ToList()
                };

                tableInfo.AnalysisName();

                return tableInfo;
            }).ToList();
        }
    }
}

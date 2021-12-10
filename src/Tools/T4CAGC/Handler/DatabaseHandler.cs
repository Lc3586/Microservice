using FreeSql.DatabaseModel;
using Microservice.Library.Container;
using Microservice.Library.Extension;
using Microservice.Library.FreeSql.Gen;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Config config)
        {
            Config = config;
            IFreeSqlProvider = AutofacHelper.GetService<IFreeSqlProvider>();
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
        /// 解析数据库
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

            var tableInfoDic = tables.ToDictionary(k => k, v => GetTableInfo(v));

            RelationshipAnalyse(tableInfoDic);

            return tableInfoDic.Values.ToList();
        }

        /// <summary>
        /// 获取表信息
        /// </summary>
        /// <param name="dbTable">数据库表</param>
        /// <returns></returns>
        static TableInfo GetTableInfo(DbTableInfo dbTable)
        {
            var tableInfo = new TableInfo
            {
                Name = dbTable.Name,
                FreeSql = true,
                Remark = dbTable.Comment,
                Fields = dbTable.Columns
                        .OrderBy(column => column.Position)
                        .Select(column =>
                        {
                            var fieldInfo = new FieldInfo
                            {
                                Name = column.Name,
                                Remark = column.Coment,
                                //DbType = column.DbTypeText,
                                Type = column.CsType.Name,
                                CsType = column.CsType,
                                CsTypeKeyword = column.CsType.Name.GetCsTypeKeyword(),
                                Primary = column.IsPrimary,
                                Nullable = column.IsNullable,
                                Length = column.MaxLength,
                                //Scale = 
                            };

                            return fieldInfo;
                        })
                        .ToList()
            };

            tableInfo.AnalysisName();

            IndexAnalyse(dbTable, tableInfo);

            return tableInfo;
        }

        /// <summary>
        /// 分析索引信息
        /// </summary>
        /// <param name="dbTable">数据库表</param>
        /// <param name="tableInfo">表信息</param>
        static void IndexAnalyse(DbTableInfo dbTable, TableInfo tableInfo)
        {
            if (!dbTable.Indexes.Any_Ex())
                return;

            dbTable.Indexes?.ForEach(o =>
            {
                o.Columns.ForEach(p =>
                {
                    var field = tableInfo.Fields.FirstOrDefault(q => string.Equals(p.Column.Name, q.Name, StringComparison.OrdinalIgnoreCase));
                    if (field == default)
                        return;

                    field.IndexName = o.Name;
                    field.Index = p.IsDesc ? IndexType.DESC : IndexType.ASC;
                });
            });
        }

        /// <summary>
        /// 分析关联信息
        /// </summary>
        /// <param name="tableInfoDic">数据库表,表信息</param>
        static void RelationshipAnalyse(Dictionary<DbTableInfo, TableInfo> tableInfoDic)
        {
            tableInfoDic.ForEach(x =>
            {
                var dbTable = x.Key;
                var tableInfo = x.Value;

                if (!dbTable.Foreigns.Any_Ex())
                    return;

                //此处判断存在问题
                if (!dbTable.Columns.Any_Ex(o => !o.IsPrimary && !dbTable.Foreigns.Any_Ex(p => p.Columns.Any_Ex(q => q.Name == o.Name))))
                    tableInfo.RelationshipTable = true;

                dbTable.Foreigns.ForEach(o =>
                {
                    o.Columns.ForEach(p =>
                    {
                        //一对一关联
                        tableInfo.Fields.Add(new FieldInfo
                        {
                            Name = o.ReferencedTable.Name == o.Table.Name ? p.Name.Replace("id", "", StringComparison.OrdinalIgnoreCase) : o.ReferencedTable.Name,
                            Remark = o.ReferencedTable.Comment,
                            Virtual = true,
                            FK = true,
                            KValue = p.Name,
                            Bind = o.ReferencedTable.Name
                        });

                        if (o.Table.Name == o.ReferencedTable.Name)
                        {
                            //树形结构
                            tableInfo.Fields.Add(new FieldInfo
                            {
                                Name = $"{p.Name.Replace("id", "", StringComparison.OrdinalIgnoreCase)}_{o.ReferencedTable.Name}s",
                                Remark = $"子级{o.ReferencedTable.Comment}",
                                Virtual = true,
                                FRK = true,
                                KValue = p.Name,
                                Bind = o.ReferencedTable.Name
                            });

                            tableInfo.Tree = true;
                        }
                        else if (tableInfo.RelationshipTable)
                        {
                            //多对多关联
                            dbTable.Foreigns
                                .Where(ox => ox.Table.Name != o.Table.Name && ox.ReferencedTable.Name != o.ReferencedTable.Name)
                                .ForEach(ox =>
                                {
                                    var rk_DbTable = tableInfoDic.Values.FirstOrDefault(q => q.Name == ox.ReferencedTable.Name);
                                    if (rk_DbTable == default)
                                        return;

                                    rk_DbTable.Fields.Add(new FieldInfo
                                    {
                                        Name = $"{o.ReferencedTable.Name}s",
                                        Remark = $"相关的{o.ReferencedTable.Comment}",
                                        Virtual = true,
                                        RK = true,
                                        KValue = dbTable.Name,
                                        Bind = o.ReferencedTable.Name
                                    });
                                });
                        }
                        else
                        {
                            //一对多关联
                            var frk_DbTable = tableInfoDic.Values.FirstOrDefault(q => q.Name == o.ReferencedTable.Name);
                            if (frk_DbTable == default)
                                return;

                            frk_DbTable.Fields.Add(new FieldInfo
                            {
                                Name = $"{o.Table.Name}s",
                                Remark = $"{o.ReferencedTable.Comment}相关的{o.Table.Comment}",
                                Virtual = true,
                                FRK = true,
                                KValue = $"{o.Table.Name}.{p.Name}",
                                Bind = o.Table.Name
                            });
                        }
                    });
                });
            });
        }
    }
}

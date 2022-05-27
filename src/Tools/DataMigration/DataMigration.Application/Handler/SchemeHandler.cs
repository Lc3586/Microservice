using DataMigration.Application.Extension;
using DataMigration.Application.Log;
using DataMigration.Application.Model;
using FreeSql;
using FreeSql.DatabaseModel;
using Microservice.Library.Extension;
using Microservice.Library.FreeSql.Extention;
using Microservice.Library.FreeSql.Gen;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DataMigration.Application.Handler
{
    /// <summary>
    /// 模式处理器
    /// </summary>
    public class SchemeHandler : IHandler
    {
        public SchemeHandler(Config config, IFreeSqlMultipleProvider<int> freeSqlMultipleProvider)
        {
            Config = config;
            FreeSqlMultipleProvider = freeSqlMultipleProvider;
        }

        /// <summary>
        /// 配置
        /// </summary>
        readonly Config Config;

        /// <summary>
        /// 
        /// </summary>
        readonly IFreeSqlMultipleProvider<int> FreeSqlMultipleProvider;

        /// <summary>
        /// 数据库表信息
        /// </summary>
        Dictionary<int, List<DbTableInfo>> Tables;

        /// <summary>
        /// 处理
        /// </summary>
        public void Handler()
        {
            try
            {
                FreeSqlMultipleProvider.Test();

                Sync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("目标数据库同步失败.", ex);
            }
        }

        /// <summary>
        /// 同步
        /// </summary>
        void Sync()
        {
            Logger.Log(NLog.LogLevel.Info, LogType.系统信息, "同步结构.");

            SyncStructure();

            Logger.Log(NLog.LogLevel.Info, LogType.系统信息, "同步外键.");

            SyncForeigns();

            Logger.Log(NLog.LogLevel.Info, LogType.系统信息, "同步注释.");

            SyncComment();
        }

        /// <summary>
        /// 同步结构
        /// </summary>
        void SyncStructure()
        {
            try
            {
                var entityTypes = FreeSqlMultipleProvider.GetEntityTypes(
                    0,
                    (Config.TableMatch?.ContainsKey(OperationType.All) == true ? Config.TableMatch[OperationType.All] : new List<string>())
                    .Concat(Config.TableMatch?.ContainsKey(OperationType.Schema) == true ? Config.TableMatch[OperationType.Schema] : new List<string>())
                    .ToList(),
                    (Config.Tables?.ContainsKey(OperationType.All) == true ? Config.Tables[OperationType.All] : new List<string>())
                    .Concat(Config.Tables?.ContainsKey(OperationType.Schema) == true ? Config.Tables[OperationType.Schema] : new List<string>())
                    .ToList(),
                    (Config.ExclusionTables?.ContainsKey(OperationType.All) == true ? Config.ExclusionTables[OperationType.All] : new List<string>())
                    .Concat(Config.ExclusionTables?.ContainsKey(OperationType.Schema) == true ? Config.ExclusionTables[OperationType.Schema] : new List<string>())
                    .ToList());

                if (entityTypes.Any_Ex())
                {
                    entityTypes.ForEach(x =>
                    {
                        try
                        {
                            FreeSqlMultipleProvider.GetOrm(Config.SameDb ? 0 : 1).CodeFirst.SyncStructure(x);
                        }
                        catch (Exception ex)
                        {
                            throw new ApplicationException($"同步{x.Name}实体类结构失败", ex);
                        }
                    });
                }

                //FreeSqlMultipleProvider.SyncStructure(1);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("同步结构失败.", ex);
            }
        }

        /// <summary>
        /// 获取数据库表
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        List<DbTableInfo> GetTables(int key)
        {
            if (Tables == null)
                Tables = new Dictionary<int, List<DbTableInfo>>();

            if (!Tables.ContainsKey(key))
                Tables.Add(key, FreeSqlMultipleProvider.GetTablesByDatabase(
                         key,
                         (Config.TableMatch?.ContainsKey(OperationType.All) == true ? Config.TableMatch[OperationType.All] : new List<string>())
                         .Concat(Config.TableMatch?.ContainsKey(OperationType.Schema) == true ? Config.TableMatch[OperationType.Schema] : new List<string>())
                         .ToList(),
                         (Config.Tables?.ContainsKey(OperationType.All) == true ? Config.Tables[OperationType.All] : new List<string>())
                         .Concat(Config.Tables?.ContainsKey(OperationType.Schema) == true ? Config.Tables[OperationType.Schema] : new List<string>())
                         .ToList(),
                         (Config.ExclusionTables?.ContainsKey(OperationType.All) == true ? Config.ExclusionTables[OperationType.All] : new List<string>())
                         .Concat(Config.ExclusionTables?.ContainsKey(OperationType.Schema) == true ? Config.ExclusionTables[OperationType.Schema] : new List<string>())
                         .ToList()));

            return Tables[key];
        }

        /// <summary>
        /// 同步外键
        /// </summary>
        void SyncForeigns()
        {
            try
            {
                var tables_source = GetTables(0);
                var tables_target = GetTables(1);

                foreach (var table_source in tables_source)
                {
                    Logger.Log(NLog.LogLevel.Info, LogType.系统信息, $"已获取源数据库数据表: {table_source.Name}.");

                    if (!table_source.ForeignsDict.Any())
                    {
                        Logger.Log(NLog.LogLevel.Info, LogType.系统信息, "已忽略: 源数据库数据表未检测到任何外键.");
                        continue;
                    }

                    var table_target = tables_target.FirstOrDefault(o => string.Equals(o.Name, table_source.Name, StringComparison.OrdinalIgnoreCase));

                    if (table_target == null)
                    {
                        Logger.Log(NLog.LogLevel.Info, LogType.系统信息, "已忽略: 目标数据库数据表不存在.");
                        continue;
                    }

                    foreach (var foreign in table_source.ForeignsDict)
                    {
                        Logger.Log(NLog.LogLevel.Info, LogType.系统信息, $"检测到外键: {foreign.Key}({foreign.Value.Table.Name}.{string.Join(",", foreign.Value.Columns.Select(o => o.Name))} => {foreign.Value.ReferencedTable.Name}.{string.Join(",", foreign.Value.ReferencedColumns.Select(o => o.Name))}).");

                        if (table_target.ForeignsDict.ContainsKey(foreign.Key)
                            || table_target.ForeignsDict.Values.Any(o =>
                                string.Equals(o.Table.Name, foreign.Value.Table.Name, StringComparison.OrdinalIgnoreCase)
                                && string.Equals(o.ReferencedTable.Name, foreign.Value.ReferencedTable.Name, StringComparison.OrdinalIgnoreCase)
                                && o.Table.Columns.Count == o.Table.Columns.Count(p => foreign.Value.Table.Columns.Any(q => q.Name == p.Name))
                                && o.ReferencedTable.Columns.Count == o.ReferencedTable.Columns.Count(p => foreign.Value.ReferencedTable.Columns.Any(q => q.Name == p.Name))))
                        {
                            Logger.Log(NLog.LogLevel.Info, LogType.系统信息, "已忽略: 目标数据库数据表已存在该外键.");
                            continue;
                        }

                        var character = FreeSqlMultipleProvider.GetOrm(Config.SameDb ? 0 : 1).Ado.GetCharacter();
                        var fk_table = FreeSqlMultipleProvider.GetOrm(Config.SameDb ? 0 : 1).Ado.GetDatabaseTableName(foreign.Value.Table);
                        var fk_referencedTable = FreeSqlMultipleProvider.GetOrm(Config.SameDb ? 0 : 1).Ado.GetDatabaseTableName(foreign.Value.ReferencedTable);
                        var fk_name = foreign.Key;
                        //var fk_name = $"{character}fk_{foreign.Value.Table.Name}_{foreign.Value.Columns[0].Name}_{foreign.Value.ReferencedTable.Name}_{foreign.ReferencedColumns[0].Name}{character}";

                        //var fk_drop_sql = Config.TargetDataType == FreeSql.DataType.MySql || Config.TargetDataType == FreeSql.DataType.OdbcMySql
                        //    ? $"ALTER TABLE {fk_table} DROP FOREIGN KEY {fk_name}"
                        //    : $"ALTER TABLE {fk_table} DROP CONSTRAINT {fk_name}";

                        var table_fk_referenced = tables_target.FirstOrDefault(o => string.Equals(o.Name, fk_referencedTable, StringComparison.OrdinalIgnoreCase));

                        if (table_fk_referenced == null)
                        {
                            Logger.Log(NLog.LogLevel.Info, LogType.系统信息, "已忽略: 未找到目标数据库指定的外键关联表，可能已配置为不处理.");
                            continue;
                        }

                        try
                        {
                            ////移除旧有外键
                            //if (orm_target.Ado.ExecuteNonQuery(fk_drop_sql) < 0)
                            //    throw new ApplicationException($"移除已有外键失败: {fk_drop_sql}.");

                            //添加外键
                            var fk_add_sql = $"ALTER TABLE {fk_table} ADD CONSTRAINT {fk_name} FOREIGN KEY ({character}{foreign.Value.Columns[0].Name}{character}) REFERENCES {fk_referencedTable} ({character}{foreign.Value.ReferencedColumns[0].Name}{character})";
                            if (FreeSqlMultipleProvider.GetOrm(Config.SameDb ? 0 : 1).Ado.ExecuteNonQuery(fk_add_sql) < 0)
                                throw new ApplicationException($"执行sql失败: {fk_add_sql}, 请检查数据库版本是否支持SQL-92中移除&添加外键的语法.");

                            Logger.Log(NLog.LogLevel.Info, LogType.系统信息, "已添加.");
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(NLog.LogLevel.Warn, LogType.警告信息, $"添加外键失败: {table_target.Name}.", null, ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("同步外键失败.", ex);
            }
        }

        /// <summary>
        /// 同步注释
        /// </summary>
        void SyncComment()
        {
            try
            {
                var tables_source = GetTables(0);
                var tables_target = GetTables(1);

                foreach (var table_source in tables_source)
                {
                    Logger.Log(NLog.LogLevel.Info, LogType.系统信息, $"已获取源数据库数据表: {table_source.Name}.");

                    var table_target = tables_target.FirstOrDefault(o => string.Equals(o.Name, table_source.Name, StringComparison.OrdinalIgnoreCase));

                    if (table_target == null)
                    {
                        Logger.Log(NLog.LogLevel.Info, LogType.系统信息, "已忽略: 目标数据库数据表不存在.");
                        continue;
                    }

                    try
                    {
                        ModifyComment(table_source, table_target);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(NLog.LogLevel.Warn, LogType.警告信息, $"添加注释失败: {table_target.Name}.", null, ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("同步注释失败.", ex);
            }
        }

        /// <summary>
        /// 编辑表注释
        /// </summary>
        /// <param name="dbTable_source">来源表信息</param>
        /// <param name="dbTable_target">目标表信息</param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        void ModifyComment(DbTableInfo dbTable_source, DbTableInfo dbTable_target)
        {
            try
            {
                switch (Config.TargetDataType)
                {
                    case DataType.MySql:
                    case DataType.OdbcMySql:
                        ModifyComment_MySql(dbTable_source, dbTable_target);
                        break;
                    case DataType.SqlServer:
                    case DataType.OdbcSqlServer:
                        ModifyComment_SqlServer(dbTable_source, dbTable_target);
                        break;
                    case DataType.Oracle:
                    case DataType.OdbcOracle:
                        ModifyComment_Oracle(dbTable_source, dbTable_target);
                        break;
                    case DataType.Dameng:
                    case DataType.OdbcDameng:
                        ModifyComment_Dameng(dbTable_source, dbTable_target);
                        break;
                    case DataType.PostgreSQL:
                    case DataType.OdbcPostgreSQL:
                        ModifyComment_PostgreSQL(dbTable_source, dbTable_target);
                        break;
                    case DataType.Sqlite:
                    case DataType.Odbc:
                    case DataType.MsAccess:
                    case DataType.OdbcKingbaseES:
                    case DataType.ShenTong:
                    case DataType.KingbaseES:
                    case DataType.Firebird:
                    case DataType.Custom:
                    default:
                        throw new ApplicationException($"不支持此数据库类型: {Config.TargetDataType}.");
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("获取编辑表注释SQL语句失败.", ex);
            }
        }

        /// <summary>
        /// 编辑注释（MySql）
        /// </summary>
        /// <param name="dbTable_source">来源表信息</param>
        /// <param name="dbTable_target">目标表信息</param>
        /// <returns></returns>
        void ModifyComment_MySql(DbTableInfo dbTable_source, DbTableInfo dbTable_target)
        {
            var character = FreeSqlMultipleProvider.GetOrm(Config.SameDb ? 0 : 1).Ado.GetCharacter();

            var sql = $"ALTER TABLE {FreeSqlMultipleProvider.GetOrm(Config.SameDb ? 0 : 1).Ado.GetDatabaseTableName(dbTable_target)} ";

            var columns_comment = string.Join(
                ",",
                dbTable_target.Columns
                .Select(x => new { columns_target = x, columns_source = dbTable_source.Columns.Find(y => y.Name.Equals(x.Name, StringComparison.OrdinalIgnoreCase)) })
                .Where(c => !c.columns_source.Comment.IsNullOrWhiteSpace())
                .Select(c =>
                {
                    Logger.Log(NLog.LogLevel.Info, LogType.系统信息, $"检测到列注释: {c.columns_source.Comment}.");
                    return $"MODIFY COLUMN {character}{c.columns_target.Name}{character} {c.columns_target.DbTypeTextFull} {(c.columns_target.IsNullable ? "NULL" : "NOT NULL")} {(c.columns_target.DefaultValue.IsNullOrWhiteSpace() ? c.columns_target.IsNullable ? "DEFAULT NULL" : "" : $"DEFAULT {c.columns_target.DefaultValue}")} COMMENT '{c.columns_source.Comment}'";
                }));

            if (columns_comment.IsNullOrWhiteSpace() && dbTable_source.Comment.IsNullOrWhiteSpace())
                return;

            if (!dbTable_source.Comment.IsNullOrWhiteSpace())
            {
                Logger.Log(NLog.LogLevel.Info, LogType.系统信息, $"检测到表注释: {dbTable_source.Comment}.");
                sql += $"{(columns_comment.IsNullOrWhiteSpace() ? "Add" : $"{columns_comment},")} COMMENT = '{dbTable_source.Comment}'";
            }

            ExecModifyCommentSQL(sql);
        }

        /// <summary>
        /// 编辑注释（SqlServer）
        /// </summary>
        /// <param name="dbTable_source">来源表信息</param>
        /// <param name="dbTable_target">目标表信息</param>
        /// <returns></returns>
        void ModifyComment_SqlServer(DbTableInfo dbTable_source, DbTableInfo dbTable_target)
        {
            //var schema = dbTable.Schema.IsNullOrWhiteSpace() ? FreeSqlMultipleProvider.GetOrm(Config.SameDb ? 0 : 1).Ado.QuerySingle<string>($"SELECT table_schema FROM information_schema.tables WHERE table_name = '{dbTable.Name}'") : dbTable.Schema;
            var schema = dbTable_target.Schema;

            if (!dbTable_source.Comment.IsNullOrWhiteSpace())
            {
                Logger.Log(NLog.LogLevel.Info, LogType.系统信息, $"检测到表注释: {dbTable_source.Comment}.");

                var sql = @$"
IF ((SELECT COUNT(*) FROM ::fn_listextendedproperty('MS_Description',
'SCHEMA', N'{schema}',
'TABLE', N'{dbTable_target.Name}', NULL, NULL)) > 0)
    EXEC sp_updateextendedproperty
'MS_Description', N'{dbTable_target.Comment}',
'SCHEMA', N'{schema}',
'TABLE', N'{dbTable_target.Name}'
ELSE EXEC sp_addextendedproperty
'MS_Description', N'{dbTable_target.Comment}',
'SCHEMA', N'{schema}',
'TABLE', N'{dbTable_target.Name}'";

                ExecModifyCommentSQL(sql);
            }

            dbTable_target.Columns
                .ForEach(x =>
                {
                    DbColumnInfo columns_source = dbTable_source.Columns.Find(y => y.Name.Equals(x.Name, StringComparison.OrdinalIgnoreCase));

                    if (columns_source.Comment.IsNullOrWhiteSpace())
                        return;

                    Logger.Log(NLog.LogLevel.Info, LogType.系统信息, $"检测到列注释: {columns_source.Comment}.");

                    var sql = $@"
IF ((SELECT COUNT(*) FROM ::fn_listextendedproperty('MS_Description',
'SCHEMA', N'{schema}',
'TABLE', N'{dbTable_target.Name}',
'COLUMN', N'{x.Name}')) > 0)
    EXEC sp_updateextendedproperty
'MS_Description', N'{columns_source.Comment}',
'SCHEMA', N'{schema}',
'TABLE', N'{dbTable_target.Name}',
'COLUMN', N'{x.Name}'
ELSE
    EXEC sp_addextendedproperty
'MS_Description', N'{columns_source.Comment}',
'SCHEMA', N'{schema}',
'TABLE', N'{dbTable_target.Name}',
'COLUMN', N'{x.Name}'";

                    ExecModifyCommentSQL(sql);
                });
        }

        /// <summary>
        /// 编辑注释（Oracle）
        /// </summary>
        /// <param name="dbTable_source">来源表信息</param>
        /// <param name="dbTable_target">目标表信息</param>
        /// <returns></returns>
        void ModifyComment_Oracle(DbTableInfo dbTable_source, DbTableInfo dbTable_target)
        {
            //var schema = dbTable.Schema.IsNullOrWhiteSpace() ? FreeSqlMultipleProvider.GetOrm(Config.SameDb ? 0 : 1).Ado.QuerySingle<string>($"SELECT OWNER FROM sys.dba_tables WHERE table_name = '{dbTable.Name}'") : dbTable.Schema;
            var schema = dbTable_target.Schema;

            if (!dbTable_source.Comment.IsNullOrWhiteSpace())
            {
                Logger.Log(NLog.LogLevel.Info, LogType.系统信息, $"检测到表注释: {dbTable_source.Comment}.");

                var sql = $"COMMENT ON TABLE {(schema.IsNullOrWhiteSpace() ? "" : "\"{schema}\".")}\"{dbTable_target.Name}\" IS '{dbTable_source.Comment}'";

                ExecModifyCommentSQL(sql);
            }

            dbTable_target.Columns
                .ForEach(x =>
                {
                    DbColumnInfo columns_source = dbTable_source.Columns.Find(y => y.Name.Equals(x.Name, StringComparison.OrdinalIgnoreCase));

                    if (columns_source.Comment.IsNullOrWhiteSpace())
                        return;

                    Logger.Log(NLog.LogLevel.Info, LogType.系统信息, $"检测到列注释: {columns_source.Comment}.");

                    var sql = $"COMMENT ON COLUMN {(schema.IsNullOrWhiteSpace() ? "" : "\"{schema}\".")}\"{dbTable_target.Name}\".\"{x.Name}\" IS '{columns_source.Comment}'";

                    ExecModifyCommentSQL(sql);
                });
        }

        /// <summary>
        /// 编辑注释（Dameng）
        /// </summary>
        /// <param name="dbTable_source">来源表信息</param>
        /// <param name="dbTable_target">目标表信息</param>
        /// <returns></returns>
        void ModifyComment_Dameng(DbTableInfo dbTable_source, DbTableInfo dbTable_target)
        {
            //SQL语句和Orcale一样
            ModifyComment_Oracle(dbTable_source, dbTable_target);
        }

        /// <summary>
        /// 编辑注释（PostgreSQL）
        /// </summary>
        /// <param name="dbTable_source">来源表信息</param>
        /// <param name="dbTable_target">目标表信息</param>
        /// <returns></returns>
        void ModifyComment_PostgreSQL(DbTableInfo dbTable_source, DbTableInfo dbTable_target)
        {
            //dbTable.Schema = dbTable.Schema.IsNullOrWhiteSpace() ? FreeSqlMultipleProvider.GetOrm(Config.SameDb ? 0 : 1).Ado.QuerySingle<string>($"SELECT table_schema FROM information_schema.tables WHERE table_name = '{dbTable.Name}'") : dbTable.Schema;

            //SQL语句和Orcale一样
            ModifyComment_Oracle(dbTable_source, dbTable_target);
        }

        /// <summary>
        /// 执行编辑注释的SQL语句
        /// </summary>
        /// <param name="sql"></param>
        /// <exception cref="ApplicationException"></exception>
        void ExecModifyCommentSQL(string sql)
        {
            try
            {
                FreeSqlMultipleProvider.GetOrm(Config.SameDb ? 0 : 1).Ado.QuerySingle<object>(sql);

                Logger.Log(NLog.LogLevel.Info, LogType.系统信息, "已更新注释.");
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"执行sql失败: {sql}.", ex);
            }
        }
    }
}

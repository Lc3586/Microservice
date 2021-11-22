using DataMigration.Application.Extension;
using DataMigration.Application.Log;
using DataMigration.Application.Model;
using FreeSql.DatabaseModel;
using Microservice.Library.Extension;
using Microservice.Library.FreeSql.Extention;
using Microservice.Library.FreeSql.Gen;
using System;
using System.Collections.Generic;
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
                    Config.TableMatch?.ContainsKey(OperationType.Schema) == true ? Config.TableMatch[OperationType.Schema] : null,
                    Config.Tables?.ContainsKey(OperationType.Schema) == true ? Config.Tables[OperationType.Schema] : null,
                    Config.ExclusionTables?.ContainsKey(OperationType.Schema) == true ? Config.ExclusionTables[OperationType.Schema] : null);

                if (entityTypes.Any_Ex())
                    FreeSqlMultipleProvider.GetOrm(1).CodeFirst.SyncStructure(entityTypes.ToArray());

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
                         0,
                         Config.TableMatch?.ContainsKey(OperationType.Schema) == true ? Config.TableMatch[OperationType.Schema] : null,
                         Config.Tables?.ContainsKey(OperationType.Schema) == true ? Config.Tables[OperationType.Schema] : null,
                         Config.ExclusionTables?.ContainsKey(OperationType.Schema) == true ? Config.ExclusionTables[OperationType.Schema] : null));

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

                    var table_target = tables_target.FirstOrDefault(o => string.Equals(o.Name, table_source.Name, StringComparison.OrdinalIgnoreCase));

                    if (table_target == null)
                    {
                        Logger.Log(NLog.LogLevel.Info, LogType.系统信息, "已忽略: 目标数据库数据表不存在.");
                        continue;
                    }

                    if (!table_source.ForeignsDict.Any())
                    {
                        Logger.Log(NLog.LogLevel.Info, LogType.系统信息, "已忽略: 源数据库数据表未检测到任何外键.");
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

                        var character = FreeSqlMultipleProvider.GetOrm(1).Ado.GetCharacter();
                        var fk_table = FreeSqlMultipleProvider.GetOrm(1).Ado.GetDatabaseTableName(foreign.Value.Table);
                        var fk_referencedTable = FreeSqlMultipleProvider.GetOrm(1).Ado.GetDatabaseTableName(foreign.Value.ReferencedTable);
                        var fk_name = foreign.Key;
                        //var fk_name = $"{character}fk_{foreign.Value.Table.Name}_{foreign.Value.Columns[0].Name}_{foreign.Value.ReferencedTable.Name}_{foreign.ReferencedColumns[0].Name}{character}";

                        //var fk_drop_sql = Config.TargetDataType == FreeSql.DataType.MySql || Config.TargetDataType == FreeSql.DataType.OdbcMySql
                        //    ? $"ALTER TABLE {fk_table} DROP FOREIGN KEY {fk_name}"
                        //    : $"ALTER TABLE {fk_table} DROP CONSTRAINT {fk_name}";

                        try
                        {
                            ////移除旧有外键
                            //if (orm_target.Ado.ExecuteNonQuery(fk_drop_sql) < 0)
                            //    throw new ApplicationException($"移除已有外键失败: {fk_drop_sql}.");

                            //添加外键
                            var fk_add_sql = $"ALTER TABLE {fk_table} ADD CONSTRAINT {fk_name} FOREIGN KEY ({character}{foreign.Value.Columns[0].Name}{character}) REFERENCES {fk_referencedTable} ({character}{foreign.Value.ReferencedColumns[0].Name}{character})";
                            if (FreeSqlMultipleProvider.GetOrm(1).Ado.ExecuteNonQuery(fk_add_sql) < 0)
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

                    var character = FreeSqlMultipleProvider.GetOrm(1).Ado.GetCharacter();

                    var comment_add_sql = string.Empty;

                    var columns_comment = string.Join(",", table_source.Columns
                        .Where(c => !c.Coment.IsNullOrWhiteSpace())
                        .Select(c =>
                        {
                            Logger.Log(NLog.LogLevel.Info, LogType.系统信息, $"检测到列注释: {c.Coment}.");
                            return $"MODIFY COLUMN {character}{c.Name}{character} {c.DbTypeTextFull} {(c.IsNullable ? "NULL" : "NOT NULL")} {(c.DefaultValue == null ? c.IsNullable ? "DEFAULT NULL" : "" : $"DEFAULT {c.DefaultValue}")} COMMENT '{c.Coment}'";
                        }));

                    if (table_target.Comment.IsNullOrWhiteSpace() && columns_comment.IsNullOrWhiteSpace())
                    {
                        Logger.Log(NLog.LogLevel.Info, LogType.系统信息, "已忽略: 源数据库数据表未检测到任何注释.");
                        continue;
                    }

                    comment_add_sql = $"ALTER TABLE {FreeSqlMultipleProvider.GetOrm(1).Ado.GetDatabaseTableName(table_source)} " + columns_comment;

                    if (columns_comment.IsNullOrWhiteSpace())
                        comment_add_sql += "Add";

                    if (!table_target.Comment.IsNullOrWhiteSpace())
                    {
                        Logger.Log(NLog.LogLevel.Info, LogType.系统信息, $"检测到表注释: {table_target.Comment}.");

                        comment_add_sql += $"COMMENT = '{table_target.Comment}'";
                    }

                    try
                    {
                        if (FreeSqlMultipleProvider.GetOrm(1).Ado.ExecuteNonQuery(comment_add_sql) < 0)
                            throw new ApplicationException($"执行sql失败: {comment_add_sql}, 请检查数据库版本是否支持SQL-92中添加注释的语法.");

                        Logger.Log(NLog.LogLevel.Info, LogType.系统信息, "已添加.");
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
    }
}

using DataMigration.Application.Log;
using DataMigration.Application.Model;
using Microservice.Library.FreeSql.Gen;
using System;
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
        }

        /// <summary>
        /// 同步结构
        /// </summary>
        void SyncStructure()
        {
            try
            {
                FreeSqlMultipleProvider.SyncStructure(1);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("同步结构失败.", ex);
            }
        }

        /// <summary>
        /// 同步外键
        /// </summary>
        void SyncForeigns()
        {
            try
            {
                var orm_source = FreeSqlMultipleProvider.GetFreeSql(0);
                var orm_target = FreeSqlMultipleProvider.GetFreeSql(1);

                var tables = orm_source.DbFirst.GetTablesByDatabase();
                foreach (var table in tables)
                {
                    Logger.Log(NLog.LogLevel.Info, LogType.系统信息, $"已获取数据表: {table.Name}.");

                    foreach (var foreign in table.ForeignsDict)
                    {
                        Logger.Log(NLog.LogLevel.Info, LogType.系统信息, $"检测到外键: {foreign.Key}({foreign.Value.Table.Name}.{string.Join(",", foreign.Value.Columns.Select(o => o.Name))} => {foreign.Value.ReferencedTable.Name}.{string.Join(",", foreign.Value.ReferencedColumns.Select(o => o.Name))}).");

                        try
                        {
                            switch (Config.TargetDataType)
                            {
                                case FreeSql.DataType.Odbc:
                                case FreeSql.DataType.MySql:
                                case FreeSql.DataType.OdbcMySql:
                                case FreeSql.DataType.SqlServer:
                                case FreeSql.DataType.OdbcSqlServer:
                                case FreeSql.DataType.Oracle:
                                case FreeSql.DataType.OdbcOracle:
                                case FreeSql.DataType.PostgreSQL:
                                case FreeSql.DataType.OdbcPostgreSQL:
                                case FreeSql.DataType.MsAccess:
                                case FreeSql.DataType.Dameng:
                                case FreeSql.DataType.OdbcDameng:
                                case FreeSql.DataType.KingbaseES:
                                case FreeSql.DataType.OdbcKingbaseES:
                                case FreeSql.DataType.ShenTong:
                                case FreeSql.DataType.Firebird:
                                default:
                                    var character = Config.TargetDataType == FreeSql.DataType.MySql || Config.TargetDataType == FreeSql.DataType.OdbcMySql
                                        ? '`'
                                        : '"';
                                    var fk_table = $"{character}{(new[] { "public", "dbo" }.Contains(foreign.Value.Table.Schema) ? "" : foreign.Value.Table.Schema)}{character}.{character}{foreign.Value.Table.Name}{character}".Replace($"{character}{character}.", "");
                                    var fk_referencedTable = $"{character}{(new[] { "public", "dbo" }.Contains(foreign.Value.ReferencedTable.Schema) ? "" : foreign.Value.ReferencedTable.Schema)}{character}.{character}{foreign.Value.ReferencedTable.Name}{character}".Replace($"{character}{character}.", "");
                                    var fk_name = foreign.Key;
                                    //var fk_name = $"{character}fk_{foreign.Value.Table.Name}_{foreign.Value.Columns[0].Name}_{foreign.Value.ReferencedTable.Name}_{foreign.ReferencedColumns[0].Name}{character}";

                                    ////移除旧有外键
                                    //var fk_drop_sql = Config.TargetDataType == FreeSql.DataType.MySql || Config.TargetDataType == FreeSql.DataType.OdbcMySql
                                    //    ? $"ALTER TABLE {fk_table} DROP FOREIGN KEY {fk_name}"
                                    //    : $"ALTER TABLE {fk_table} DROP CONSTRAINT {fk_name}";

                                    //if (orm_target.Ado.ExecuteNonQuery(fk_drop_sql) < 0)
                                    //    throw new ApplicationException($"移除已有外键失败: {fk_drop_sql}.");

                                    //添加外键
                                    var fk_add_sql = $"ALTER TABLE {fk_table} ADD CONSTRAINT {fk_name} FOREIGN KEY ({character}{foreign.Value.Columns[0].Name}{character}) REFERENCES {fk_referencedTable} ({character}{foreign.Value.ReferencedColumns[0].Name}{character})";
                                    if (orm_target.Ado.ExecuteNonQuery(fk_add_sql) < 0)
                                        throw new ApplicationException($"添加外键失败: {fk_add_sql}.");

                                    break;
                                case FreeSql.DataType.Sqlite:
                                    throw new ApplicationException("Sqlite不支持SQL-92中移除&添加外键的语法.");
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(NLog.LogLevel.Warn, LogType.警告信息, $"添加外键失败: {table.Name}.", null, ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("同步外键失败.", ex);
            }
        }
    }
}

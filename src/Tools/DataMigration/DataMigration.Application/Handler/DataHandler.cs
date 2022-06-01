using DataMigration.Application.Extension;
using DataMigration.Application.Log;
using DataMigration.Application.Model;
using FreeSql;
using FreeSql.DatabaseModel;
using Microservice.Library.Extension;
using Microservice.Library.FreeSql.Extention;
using Microservice.Library.FreeSql.Gen;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DataMigration.Application.Handler
{
    /// <summary>
    /// 数据处理器
    /// </summary>
    public class DataHandler : IHandler
    {
        public DataHandler(Config config, IFreeSqlMultipleProvider<int> freeSqlMultipleProvider)
        {
            Config = config;
            FreeSqlMultipleProvider = freeSqlMultipleProvider;

            GetInsertMethodInfo();
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
        /// 插入方法名称
        /// </summary>
        string InsertMethodName;

        /// <summary>
        /// 工作单元
        /// </summary>
        IRepositoryUnitOfWork UOW;

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
                throw new ApplicationException("目标数据库同步数据失败.", ex);
            }
        }

        /// <summary>
        /// 同步
        /// </summary>
        void Sync()
        {
            Logger.Log(NLog.LogLevel.Info, LogType.系统信息, "在目标数据库开启事务.");


            UOW = FreeSqlMultipleProvider.GetOrm(Config.SameDb ? 0 : 1).CreateUnitOfWork();
            try
            {
                SyncData();
                UOW.Commit();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("同步数据失败.", ex);
            }
            finally
            {
                UOW.Rollback();
                UOW.Dispose();
            }
        }

        /// <summary>
        /// 同步数据
        /// </summary>
        void SyncData()
        {
            Logger.Log(NLog.LogLevel.Info, LogType.系统信息, "禁用外键约束.");

            DisableForeignKeyCheck();

            Logger.Log(NLog.LogLevel.Info, LogType.系统信息, "同步数据.");

            SyncDataByDatabase();

            Logger.Log(NLog.LogLevel.Info, LogType.系统信息, "启用外键约束.");

            EnableForeignKeyCheck();
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
                         .Concat(Config.TableMatch?.ContainsKey(OperationType.Data) == true ? Config.TableMatch[OperationType.Data] : new List<string>())
                         .ToList(),
                         (Config.Tables?.ContainsKey(OperationType.All) == true ? Config.Tables[OperationType.All] : new List<string>())
                         .Concat(Config.Tables?.ContainsKey(OperationType.Data) == true ? Config.Tables[OperationType.Data] : new List<string>())
                         .ToList(),
                         (Config.ExclusionTables?.ContainsKey(OperationType.All) == true ? Config.ExclusionTables[OperationType.All] : new List<string>())
                         .Concat(Config.ExclusionTables?.ContainsKey(OperationType.Data) == true ? Config.ExclusionTables[OperationType.Data] : new List<string>())
                         .ToList()));

            return Tables[key];
        }

        /// <summary>
        /// 获取数据插入方法信息
        /// </summary>
        /// <returns>(方法名, 是否批量插入方法)</returns>
        void GetInsertMethodInfo()
        {
            InsertMethodName = Config.UseBulkCopy ? Config.TargetDataType switch
            {
                DataType.MySql => nameof(FreeSqlMySqlConnectorGlobalExtensions.ExecuteMySqlBulkCopy),
                DataType.SqlServer => nameof(FreeSqlSqlServerGlobalExtensions.ExecuteSqlBulkCopy),
                DataType.PostgreSQL => nameof(FreeSqlPostgreSQLGlobalExtensions.ExecutePgCopy),
                DataType.Oracle => nameof(FreeSqlOracleGlobalExtensions.ExecuteOracleBulkCopy),
                DataType.Dameng => nameof(FreeSqlDamengGlobalExtensions.ExecuteDmBulkCopy),
                _ => null
            } : null;

            if (InsertMethodName == null)
                InsertMethodName = nameof(IInsert<object>.ExecuteAffrows);
        }

        /// <summary>
        /// 获取数据插入方法
        /// </summary>
        /// <returns>(方法, 是否异步, 是否返回行数)</returns>
        private int? Insert(IInsert<object> iInsert, Type entityType)
        {
            switch (InsertMethodName)
            {
                case nameof(FreeSqlMySqlConnectorGlobalExtensions.ExecuteMySqlBulkCopyAsync):
                    iInsert.ExecuteMySqlBulkCopyAsync().GetAwaiter().GetResult();
                    return null;
                case nameof(FreeSqlMySqlConnectorGlobalExtensions.ExecuteMySqlBulkCopy):
                    iInsert.ExecuteMySqlBulkCopy();
                    return null;
                case nameof(FreeSqlSqlServerGlobalExtensions.ExecuteSqlBulkCopyAsync):
                    iInsert.ExecuteSqlBulkCopyAsync().GetAwaiter().GetResult();
                    return null;
                case nameof(FreeSqlSqlServerGlobalExtensions.ExecuteSqlBulkCopy):
                    iInsert.ExecuteSqlBulkCopy();
                    return null;
                case nameof(FreeSqlPostgreSQLGlobalExtensions.ExecutePgCopyAsync):
                    iInsert.ExecutePgCopyAsync().GetAwaiter().GetResult();
                    return null;
                case nameof(FreeSqlPostgreSQLGlobalExtensions.ExecutePgCopy):
                    iInsert.ExecutePgCopy();
                    return null;
                case nameof(FreeSqlOracleGlobalExtensions.ExecuteOracleBulkCopy):
                    iInsert.ExecuteOracleBulkCopy();
                    return null;
                case nameof(FreeSqlDamengGlobalExtensions.ExecuteDmBulkCopy):
                    iInsert.ExecuteDmBulkCopy();
                    return null;
                case nameof(IInsert<object>.ExecuteAffrowsAsync):
                    return iInsert.ExecuteAffrowsAsync().GetAwaiter().GetResult();
                case nameof(IInsert<object>.ExecuteAffrows):
                default:
                    return iInsert.ExecuteAffrows();
            }
        }

        /// <summary>
        /// 同步数据库数据
        /// </summary>
        void SyncDataByDatabase()
        {
            var orm_source = FreeSqlMultipleProvider.GetOrm(0);
            var orm_target = UOW.Orm;

            var tables_source = GetTables(0);
            var tables_target = GetTables(1);

            var entityTypes = FreeSqlMultipleProvider.GetEntityTypes(
                0,
                (Config.TableMatch?.ContainsKey(OperationType.All) == true ? Config.TableMatch[OperationType.All] : new List<string>())
                    .Concat(Config.TableMatch?.ContainsKey(OperationType.Data) == true ? Config.TableMatch[OperationType.Data] : new List<string>())
                    .ToList(),
                    (Config.Tables?.ContainsKey(OperationType.All) == true ? Config.Tables[OperationType.All] : new List<string>())
                    .Concat(Config.Tables?.ContainsKey(OperationType.Data) == true ? Config.Tables[OperationType.Data] : new List<string>())
                    .ToList(),
                    (Config.ExclusionTables?.ContainsKey(OperationType.All) == true ? Config.ExclusionTables[OperationType.All] : new List<string>())
                    .Concat(Config.ExclusionTables?.ContainsKey(OperationType.Data) == true ? Config.ExclusionTables[OperationType.Data] : new List<string>())
                    .ToList());

            foreach (var table_source in tables_source)
            {
                Logger.Log(NLog.LogLevel.Info, LogType.系统信息, $"已获取源数据库数据表: {table_source.Name}.");

                var table_target = tables_target.FirstOrDefault(o => string.Equals(o.Name, table_source.Name, StringComparison.OrdinalIgnoreCase));

                if (table_target == null)
                {
                    Logger.Log(NLog.LogLevel.Info, LogType.系统信息, "已忽略: 目标数据库数据表不存在.");
                    continue;
                }

                var entityType = entityTypes.FirstOrDefault(o => string.Equals(o.Name, table_source.Name, StringComparison.OrdinalIgnoreCase));

                var currentPage = 1;

            retry:

                while (true)
                {
                    try
                    {
                        var iSelect = orm_source.Select<object>().AsType(entityType);

                        if (Config.UseSql?.TryGetValue(table_source.Name.ToLower(), out string sql) == true)
                        {
                            iSelect.WithSql(sql);
                            Logger.Log(NLog.LogLevel.Info, LogType.系统信息, $"使用自定义SQL查询语句: {sql}.");
                        }

                        iSelect.Page(currentPage, Config.DataPageSize);

                        var data = iSelect.ToList(false);
                        var rows = (data as ICollection).Count;

                        Logger.Log(NLog.LogLevel.Info, LogType.系统信息, $"在第{currentPage}页获取到{rows}条数据.");

                        if (rows == 0)
                            break;

                        var iInsert = orm_target.Insert<object>().AsType(entityType);

                        iInsert.AppendData(data);
                        iInsert.NoneParameter(false);

                        //#if DEBUG
                        //                        iInsert.NoneParameter(true);
                        //#endif

                        var result = Insert(iInsert, entityType);
                        rows = result ?? rows;

                        Logger.Log(NLog.LogLevel.Info, LogType.系统信息, $"已导入{rows}条数据.");

                        currentPage++;
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(NLog.LogLevel.Warn, LogType.警告信息, $"导入数据时发生异常: {table_target.Name}.", null, ex);

#if DEBUG
                        var type = ex.GetType();
                        Trace.WriteLine(type.FullName);
#endif

                        if (Config.UseBulkCopy)
                        {
                            Logger.Log(NLog.LogLevel.Warn, LogType.警告信息, $"当前环境可能不支持批量插入功能, 将尝试关闭此功能后导入数据.");

                            Config.UseBulkCopy = false;
                            GetInsertMethodInfo();
                            goto retry;
                        }

                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// 禁用外键约束
        /// </summary>
        void DisableForeignKeyCheck()
        {
            try
            {
                switch (Config.TargetDataType)
                {
                    case DataType.MySql:
                    case DataType.OdbcMySql:
                        SwitchForeignKeyCheck_MySql(true);
                        break;
                    case DataType.SqlServer:
                    case DataType.OdbcSqlServer:
                        SwitchForeignKeyCheck_SqlServer(true);
                        break;
                    case DataType.Oracle:
                    case DataType.OdbcOracle:
                        SwitchForeignKeyCheck_Oracle(true);
                        break;
                    case DataType.Dameng:
                    case DataType.OdbcDameng:
                        SwitchForeignKeyCheck_Dameng(true);
                        break;
                    case DataType.PostgreSQL:
                    case DataType.OdbcPostgreSQL:
                        SwitchForeignKeyCheck_PostgreSQL(true);
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
                throw new ApplicationException("关闭外键约束失败.", ex);
            }
        }

        /// <summary>
        /// 启用外键约束
        /// </summary>
        void EnableForeignKeyCheck()
        {
            try
            {
                switch (Config.TargetDataType)
                {
                    case DataType.MySql:
                    case DataType.OdbcMySql:
                        SwitchForeignKeyCheck_MySql(false);
                        break;
                    case DataType.SqlServer:
                    case DataType.OdbcSqlServer:
                        SwitchForeignKeyCheck_SqlServer(false);
                        break;
                    case DataType.Oracle:
                    case DataType.OdbcOracle:
                        SwitchForeignKeyCheck_Oracle(false);
                        break;
                    case DataType.Dameng:
                    case DataType.OdbcDameng:
                        SwitchForeignKeyCheck_Dameng(false);
                        break;
                    case DataType.PostgreSQL:
                    case DataType.OdbcPostgreSQL:
                        SwitchForeignKeyCheck_PostgreSQL(false);
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
                throw new ApplicationException("启用外键约束失败.", ex);
            }
        }

        /// <summary>
        /// 切换外键检查状态（MySql）
        /// </summary>
        /// <param name="disable">禁用</param>
        void SwitchForeignKeyCheck_MySql(bool disable)
        {
            var ado = UOW.Orm.Ado;
            var sql = $"SET FOREIGN_KEY_CHECKS={(disable ? '0' : '1')}";

            try
            {
                ado.ExecuteNonQuery(sql);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"执行sql失败: {sql}.", ex);
            }
        }

        /// <summary>
        /// 切换外键检查状态（SqlServer）
        /// </summary>
        /// <param name="disable">禁用</param>
        void SwitchForeignKeyCheck_SqlServer(bool disable)
        {
            var ado = UOW.Orm.Ado;
            var sql = $"EXEC sp_MSforeachtable @command1='ALTER TABLE ? {(disable ? "NOCHECK" : "CHECK")} CONSTRAINT ALL'";

            try
            {
                ado.ExecuteNonQuery(sql);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"执行sql失败: {sql}.", ex);
            }
        }

        /// <summary>
        /// 切换外键检查状态（Oracle）
        /// </summary>
        /// <param name="disable">禁用</param>
        void SwitchForeignKeyCheck_Oracle(bool disable)
        {
            var ado = UOW.Orm.Ado;
            var select_sql = @$"
SELECT 
    'ALTER TABLE ""'||TABLE_NAME||'"" {(disable ? "DISABLE" : "ENABLE")} CONSTRAINT ""'||constraint_name||'"" ' 
FROM user_constraints 
WHERE CONSTRAINT_TYPE='R' 
    OR CONSTRAINT_TYPE='C'";
            var sqls = ado.Query<string>(select_sql);
            if (!sqls.Any_Ex())
                return;

            sqls.ForEach(x =>
            {
                try
                {
                    ado.ExecuteNonQuery(x);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException($"执行sql失败: {x}.", ex);
                }
            });
        }

        /// <summary>
        /// 切换外键检查状态（Dameng）
        /// </summary>
        /// <param name="disable">禁用</param>
        void SwitchForeignKeyCheck_Dameng(bool disable)
        {
            var ado = UOW.Orm.Ado;
            var select_sql = @$"
SELECT 
    'ALTER TABLE ""'||OWNER||'"".""'||TABLE_NAME||'"" {(disable ? "DISABLE" : "ENABLE")} CONSTRAINT ""'||CONSTRAINT_NAME||'""'
FROM SYSCONS a, SYSOBJECTS b, ALL_CONS_COLUMNS c
WHERE a.ID=b.ID 
    AND a.TYPE$='F'
    AND b.NAME=c.CONSTRAINT_NAME
    AND c.OWNER NOT IN('SYS')";
            var sqls = ado.Query<string>(select_sql);
            if (!sqls.Any_Ex())
                return;

            sqls.ForEach(x =>
            {
                try
                {
                    ado.ExecuteNonQuery(x);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException($"执行sql失败: {x}.", ex);
                }
            });
        }

        /// <summary>
        /// 切换外键检查状态（PostgreSQL）
        /// </summary>
        /// <param name="disable">禁用</param>
        void SwitchForeignKeyCheck_PostgreSQL(bool disable)
        {
            var ado = UOW.Orm.Ado;
            var sql = $"SET session_replication_role='{(disable ? "replica" : "origin")}'";
            try
            {
                ado.ExecuteNonQuery(sql);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"执行sql失败: {sql}.", ex);
            }
        }
    }
}

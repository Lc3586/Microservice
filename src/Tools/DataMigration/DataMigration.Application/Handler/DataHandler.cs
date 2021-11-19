using DataMigration.Application.Extension;
using DataMigration.Application.Log;
using DataMigration.Application.Model;
using Dm;
using FreeSql;
using Microservice.Library.Extension;
using Microservice.Library.FreeSql.Extention;
using Microservice.Library.FreeSql.Gen;
using Microsoft.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

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
                throw new ApplicationException("目标数据库同步数据失败.", ex);
            }
        }

        /// <summary>
        /// 同步
        /// </summary>
        void Sync()
        {
            Logger.Log(NLog.LogLevel.Info, LogType.系统信息, "运行事务.");

            (bool success, Exception ex) = FreeSqlMultipleProvider.GetOrm(1).RunTransaction(SyncData);

            if (!success)
                throw new ApplicationException("同步数据失败.", ex);
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
        /// 同步数据库数据
        /// </summary>
        void SyncDataByDatabase()
        {
            var orm_source = FreeSqlMultipleProvider.GetOrm(0);
            var orm_target = FreeSqlMultipleProvider.GetOrm(1);

            var tables_source = FreeSqlMultipleProvider.GetTablesByDatabase(0);
            var tables_target = FreeSqlMultipleProvider.GetTablesByDatabase(1);

            var entityTypes = Extension.Extension.GetEntityTypes();

            foreach (var table_source in tables_source)
            {
                Logger.Log(NLog.LogLevel.Info, LogType.系统信息, $"已获取源数据库数据表: {table_source.Name}.");

                var table_target = tables_target.FirstOrDefault(o => string.Equals(o.Name, table_source.Name, StringComparison.OrdinalIgnoreCase));

                if (table_target == null)
                {
                    Logger.Log(NLog.LogLevel.Info, LogType.系统信息, "已忽略: 目标数据库数据表不存在.");
                    continue;
                }

                if (table_source.Name.ToLower() != "bizbatch")
                    continue;

                var entityType = entityTypes.FirstOrDefault(o => string.Equals(o.Name, table_source.Name, StringComparison.OrdinalIgnoreCase));

                var iSelect = typeof(IFreeSql)
                    .GetMethod(nameof(IFreeSql.Select), 1, Array.Empty<Type>())
                    .MakeGenericMethod(entityType)
                    .Invoke(orm_source, null);

                var sql_method = typeof(ISelect<>)
                    .MakeGenericType(entityType)
                    .GetMethod(nameof(ISelect<object>.WithSql), new Type[] { typeof(string), typeof(object) });

                var page_method = typeof(ISelect0<,>)
                    .MakeGenericType(typeof(ISelect<>).MakeGenericType(entityType), entityType)
                    .GetMethod(nameof(ISelect<object>.Page), new Type[] { typeof(int), typeof(int) });

                var toList_method = typeof(ISelect0<,>)
                    .MakeGenericType(typeof(ISelect<>).MakeGenericType(entityType), entityType)
                    .GetMethod(nameof(ISelect<object>.ToList), new Type[] { typeof(bool) });

                var iInsert = typeof(IFreeSql)
                    .GetMethod(nameof(IFreeSql.Insert), 1, Array.Empty<Type>())
                    .MakeGenericMethod(entityType)
                    .Invoke(orm_target, null);

                var appendData_method = typeof(IInsert<>)
                    .MakeGenericType(entityType)
                    .GetMethod(nameof(IInsert<object>.AppendData), new Type[] { typeof(IEnumerable<>).MakeGenericType(entityType) });


            retry:

                var (insertData_method, insertData_async, insertData_returnRows) = GetInsert(entityType);

                var currentPage = 1;

                while (true)
                {
                    try
                    {

                        if (Config.UseSql.TryGetValue(table_source.Name.ToLower(), out string sql))
                        {
                            sql_method.Invoke(iSelect, new object[] { sql, null });
                            Logger.Log(NLog.LogLevel.Info, LogType.系统信息, $"使用自定义SQL查询语句: {sql}.");
                        }

                        page_method.Invoke(iSelect, new object[] { currentPage, Config.DataPageSize });

                        var data = toList_method.Invoke(iSelect, new object[] { false });
                        var rows = (data as ICollection).Count;

                        Logger.Log(NLog.LogLevel.Info, LogType.系统信息, $"在第{currentPage}页（每页数据量{Config.DataPageSize}）获取到{rows}条数据.");

                        if (rows == 0)
                            break;

                        currentPage++;

                        appendData_method.Invoke(iInsert, new object[] { data });
                        var result = insertData_method.Invoke(iInsert, null);

                        if (insertData_async)
                        {
                            if (insertData_returnRows)
                            {
                                rows = (result as Task<int>).GetAwaiter().GetResult();
                            }
                            else
                                (result as Task).GetAwaiter().GetResult();
                        }
                        else if (insertData_returnRows)
                        {
                            rows = (int)result;
                        }

                        Logger.Log(NLog.LogLevel.Info, LogType.系统信息, $"已导入{rows}条数据.");
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(NLog.LogLevel.Warn, LogType.警告信息, $"导入数据失败: {table_target.Name}.", null, ex);

                        if (ex as NotSupportedException != null && Config.UseBulkCopy)
                        {
                            Logger.Log(NLog.LogLevel.Warn, LogType.警告信息, $"当前环境可能不支持批量插入功能, 将尝试关闭此功能后再导入数据.");

                            Config.UseBulkCopy = false;
                            goto retry;
                        }
                        break;
                    }
                }

                //orm_source.Select<object>().Page(currentPage, Config.DataPageSize).ToList();
                //orm_source.Insert<object>().AppendData(new List<object>()).ExecutePgCopy();                                
            }
        }

        /// <summary>
        /// 获取数据插入方法
        /// </summary>
        /// <returns>(方法, 是否异步, 是否返回行数)</returns>
        (MethodInfo method, bool async, bool returnRows) GetInsert(Type entityType)
        {
            (MethodInfo method, bool async, bool returnRows) insertData = default;

            if (Config.UseBulkCopy)
                switch (Config.TargetDataType)
                {
                    case DataType.MySql:
                    case DataType.OdbcMySql:
                        insertData.method = typeof(FreeSqlMySqlConnectorGlobalExtensions)
                            .GetMethod(nameof(FreeSqlMySqlConnectorGlobalExtensions.ExecuteMySqlBulkCopyAsync))
                            //?.GetMethod(nameof(FreeSqlMySqlConnectorGlobalExtensions.ExecuteMySqlBulkCopyAsync), 1, new Type[] { typeof(IInsert<>).MakeGenericType(entityType), typeof(int?), typeof(CancellationToken) })
                            .MakeGenericMethod(entityType);
                        insertData.async = true;
                        insertData.returnRows = false;
                        break;
                    case DataType.SqlServer:
                    case DataType.OdbcSqlServer:
                        insertData.method = typeof(FreeSqlSqlServerGlobalExtensions)
                            .GetMethod(nameof(FreeSqlSqlServerGlobalExtensions.ExecuteSqlBulkCopyAsync))
                            //.GetMethod(nameof(FreeSqlSqlServerGlobalExtensions.ExecuteSqlBulkCopyAsync), 1, new Type[] { typeof(IInsert<>).MakeGenericType(entityType), typeof(SqlBulkCopyOptions), typeof(int?), typeof(int?), typeof(CancellationToken) })
                            .MakeGenericMethod(entityType);
                        insertData.async = true;
                        insertData.returnRows = false;
                        break;
                    case DataType.PostgreSQL:
                    case DataType.OdbcPostgreSQL:
                        insertData.method = typeof(FreeSqlPostgreSQLGlobalExtensions)
                            .GetMethod(nameof(FreeSqlPostgreSQLGlobalExtensions.ExecutePgCopyAsync))
                            //.GetMethod(nameof(FreeSqlPostgreSQLGlobalExtensions.ExecutePgCopyAsync), 1, new Type[] { typeof(IInsert<>).MakeGenericType(entityType), typeof(CancellationToken) })
                            .MakeGenericMethod(entityType);
                        insertData.async = true;
                        insertData.returnRows = false;
                        break;
                    case DataType.Oracle:
                    case DataType.OdbcOracle:
                        insertData.method = typeof(FreeSqlOracleGlobalExtensions)
                            .GetMethod(nameof(FreeSqlOracleGlobalExtensions.ExecuteOracleBulkCopy))
                            //.GetMethod(nameof(FreeSqlOracleGlobalExtensions.ExecuteOracleBulkCopy), 1, new Type[] { typeof(IInsert<>).MakeGenericType(entityType), typeof(OracleBulkCopyOptions), typeof(int?), typeof(int?) })
                            .MakeGenericMethod(entityType);
                        insertData.async = false;
                        insertData.returnRows = false;
                        break;
                    case DataType.Dameng:
                    case DataType.OdbcDameng:
                        insertData.method = typeof(FreeSqlDamengGlobalExtensions)
                            .GetMethod(nameof(FreeSqlDamengGlobalExtensions.ExecuteDmBulkCopy))
                            //.GetMethod(nameof(FreeSqlDamengGlobalExtensions.ExecuteDmBulkCopy), 1, new Type[] { typeof(IInsert<>).MakeGenericType(entityType), typeof(DmBulkCopyOptions), typeof(int?), typeof(int?) })
                            .MakeGenericMethod(entityType);
                        insertData.async = false;
                        insertData.returnRows = false;
                        break;
                    default:
                        break;
                }

            if (insertData == default)
            {
                insertData.method = typeof(IInsert<>)
                            .MakeGenericType(entityType)
                            .GetMethod(nameof(IInsert<object>.ExecuteAffrowsAsync));
                //.GetMethod(nameof(IInsert<object>.ExecuteAffrowsAsync), new Type[] { typeof(CancellationToken) });
                insertData.async = true;
                insertData.returnRows = true;
            }

            return insertData;
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
            var ado = FreeSqlMultipleProvider.GetOrm(1).Ado;
            var sql = $"SET FOREIGN_KEY_CHECKS={(disable ? '0' : '1')}";
            if (ado.ExecuteNonQuery(sql) < 0)
                throw new ApplicationException($"执行sql失败: {sql}.");
        }

        /// <summary>
        /// 切换外键检查状态（SqlServer）
        /// </summary>
        /// <param name="disable">禁用</param>
        void SwitchForeignKeyCheck_SqlServer(bool disable)
        {
            var ado = FreeSqlMultipleProvider.GetOrm(1).Ado;
            var sql = $"EXEC sp_MSforeachtable @command1='ALTER TABLE ? {(disable ? "NOCHECK" : "CHECK")} CONSTRAINT ALL";
            if (ado.ExecuteNonQuery(sql) < 0)
                throw new ApplicationException($"执行sql失败: {sql}.");
        }

        /// <summary>
        /// 切换外键检查状态（Oracle）
        /// </summary>
        /// <param name="disable">禁用</param>
        void SwitchForeignKeyCheck_Oracle(bool disable)
        {
            var ado = FreeSqlMultipleProvider.GetOrm(1).Ado;
            var select_sql = @$"
SELECT 
    'ALTER TABLE ""'||TABLE_NAME||'"" {(disable ? "DISABLE" : "ENABLE")} CONSTRAINT ""'||constraint_name||'"" ' 
FROM user_constraints 
WHERE CONSTRAINT_TYPE='R' 
    OR CONSTRAINT_TYPE='C'";
            var sqls = ado.Query<string>(select_sql);
            if (!sqls.Any_Ex())
                return;

            var sql = string.Join(";", sqls);
            if (ado.ExecuteNonQuery(sql) < 0)
                throw new ApplicationException($"执行sql失败: {sql}.");
        }

        /// <summary>
        /// 切换外键检查状态（Dameng）
        /// </summary>
        /// <param name="disable">禁用</param>
        void SwitchForeignKeyCheck_Dameng(bool disable)
        {
            var ado = FreeSqlMultipleProvider.GetOrm(1).Ado;
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

            var sql = string.Join(";", sqls);
            if (ado.ExecuteNonQuery(sql) < 0)
                throw new ApplicationException($"执行sql失败: {sql}.");
        }

        /// <summary>
        /// 切换外键检查状态（PostgreSQL）
        /// </summary>
        /// <param name="disable">禁用</param>
        void SwitchForeignKeyCheck_PostgreSQL(bool disable)
        {
            var ado = FreeSqlMultipleProvider.GetOrm(1).Ado;
            var sql = $"SET session_replication_role='{(disable ? "replica" : "origin")}'";
            if (ado.ExecuteNonQuery(sql) < 0)
                throw new ApplicationException($"执行sql失败: {sql}.");
        }
    }
}

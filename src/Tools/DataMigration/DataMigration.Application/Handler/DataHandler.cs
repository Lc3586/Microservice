using DataMigration.Application.Extension;
using DataMigration.Application.Log;
using DataMigration.Application.Model;
using FreeSql;
using Microservice.Library.FreeSql.Extention;
using Microservice.Library.FreeSql.Gen;
using System;

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


            Logger.Log(NLog.LogLevel.Info, LogType.系统信息, "启用外键约束.");

            EnableForeignKeyCheck();
        }

        /// <summary>
        /// 禁用外键约束
        /// </summary>
        void DisableForeignKeyCheck()
        {
            try
            {
                var sql = Config.TargetDataType switch
                {
                    DataType.MySql or DataType.OdbcMySql => "SET FOREIGN_KEY_CHECKS = 0",
                    DataType.SqlServer or DataType.OdbcSqlServer => "EXEC sp_MSforeachtable @command1='alter table ?  NOCHECK constraint all",
                    DataType.Oracle or DataType.OdbcOracle => @"
SET SERVEROUTPUT ON SIZE 50000
BEGIN
for c in (select 'ALTER TABLE '||TABLE_NAME||' DISABLE CONSTRAINT '||constraint_name||' ' as v_sql from user_constraints where CONSTRAINT_TYPE='R' or CONSTRAINT_TYPE='C') loop
DBMS_OUTPUT.PUT_LINE(C.V_SQL);
begin
EXECUTE IMMEDIATE c.v_sql;
exception when others then
dbms_output.put_line(sqlerrm);
end;
end loop;
end;",
                    _ => throw new ApplicationException($"不支持此数据库类型: {Config.TargetDataType}.")
                };

                if (FreeSqlMultipleProvider.GetOrm(1).Ado.ExecuteNonQuery(sql) < 0)
                    throw new ApplicationException($"执行sql失败: {sql}.");
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
                var sql = Config.TargetDataType switch
                {
                    DataType.MySql or DataType.OdbcMySql => "SET FOREIGN_KEY_CHECKS = 1",
                    DataType.SqlServer or DataType.OdbcSqlServer => "EXEC sp_MSforeachtable @command1='alter table ?  CHECK constraint all",
                    DataType.Oracle or DataType.OdbcOracle => @"
SET SERVEROUTPUT ON SIZE 50000
begin
for c in (select 'ALTER TABLE '||TABLE_NAME||' ENABLE CONSTRAINT '||constraint_name||' ' as v_sql from user_constraints where CONSTRAINT_TYPE='R' or CONSTRAINT_TYPE='C') loop
DBMS_OUTPUT.PUT_LINE(C.V_SQL);
begin
EXECUTE IMMEDIATE c.v_sql;
exception when others then
dbms_output.put_line(sqlerrm);
end;
end loop;
end;",
                    _ => throw new ApplicationException($"不支持此数据库类型: {Config.TargetDataType}.")
                };

                if (FreeSqlMultipleProvider.GetOrm(1).Ado.ExecuteNonQuery(sql) < 0)
                    throw new ApplicationException($"执行sql失败: {sql}.");
            }
            catch (Exception ex)
            {
                throw new ApplicationException("启用外键约束失败.", ex);
            }
        }

//        ```-----启用或禁用约束存储过程
//CREATE OR REPLACE PROCEDURE FOREIGN_KEY_CHECKS(check_enable int)
//as
//declare ENABLESTR STRING;
//begin
//    if check_enable=1 then ENABLESTR = ' ENABLE CONSTRAINT '; --启用
//      elseif check_enable=0 then ENABLESTR = ' DISABLE CONSTRAINT ';--禁用
//        end if;
//    for rec in
//    (
//            select
//                    TABLE_NAME,
//                    COLUMN_NAME,
//                    CONSTRAINT_NAME,
//                     OWNER
//            from
//                    SYSCONS a   ,
//                    SYSOBJECTS b,
//                    ALL_CONS_COLUMNS c
//            where
//                    a.id        =b.id
//                and a.TYPE$     ='F' --'F'代表外键，'P'代表主键，'U'唯一索引
//                and b.name      =c.CONSTRAINT_NAME
//                and c.owner not in ('SYS')
//    )
//    loop
//             execute immediate 'alter table '||rec.OWNER||'.'||rec.TABLE_NAME||ENABLESTR||rec.CONSTRAINT_NAME;
//            --print 'alter table '||rec.OWNER||'.'||rec.TABLE_NAME||ENABLESTR||rec.CONSTRAINT_NAME;
//            commit;
//    end loop;

//        end;
//--执行存储过程
//call FOREIGN_KEY_CHECKS(1);--启用约束
//call FOREIGN_KEY_CHECKS(0);--禁用约束
//————————————————
//版权声明：本文为CSDN博主「dmdba1」的原创文章，遵循CC 4.0 BY-SA版权协议，转载请附上原文出处链接及本声明。
//原文链接：https://blog.csdn.net/fengxiaozhenjay/article/details/104560980
    }
}

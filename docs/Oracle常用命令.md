# 启动PDB数据库

> ## 查询CDB容器名称
> show con_name

> ## 启动PDB
> alter session set container=orclpdb;
> startup;

# 修改密码
> sqlplus / as sysdba
> 
> alter user sys identified by '123456';

  * sys 用户名
  * 123456 密码

# 新建数据库
> sqlplus / as sysdba

> ## 创建用户表空间
> create tablespace ADMIN datafile
>   'C:\oracle\oradata\ORCL\ADMIN.dbf' size 100M autoextend on next 10M maxsize unlimited;

> ## 创建索引表空间
> create tablespace admin_idx datafile
>   'C:\oracle\oradata\ORCL\ADMIN_IDX.dbf' size 100M autoextend on next 10M maxsize unlimited;

> ## 创建临时表空间
> create temporary tablespace admin_temp tempfile
>   'C:\oracle\oradata\ORCL\ADMIN_TEMP.dbf' size 100M autoextend on next 10M maxsize 100M;

> ## 创建用户
> create user admin identified by "-3A79-4411-8B9F-" default tablespace admin;

> ## 用户 TPPAML授权
> grant connect to admin;
> 
> grant resource to admin;
> 
> grant create table to admin;

> ## 用户 Debug、建表授权
> grant debug any procedure, debug connect session to admin;
> 
> grant create any sequence to admin;
> 
> grant create any table to admin;
> 
> grant delete any table to admin;
> 
> grant insert any table to admin;
> 
> grant select any table to admin;
> 
> grant unlimited tablespace to admin;
> 
> grant execute any procedure to admin;
> 
> grant update any table to admin;
> 
> grant create any view to admin;

> ## 用户 所有表空间下的建表授权
> grant unlimited tablespace to admin;

> ## 用户 无限制的空间限额
> alter user admin quota unlimited on admin;

> ## 用户DBA授权
> grant dba to admin

> ## 删除表空间和文件
> drop tablespace admin including contents and datafiles;

> ## 删除用户
> drop user admin cascade;
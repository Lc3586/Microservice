### #常用命令

###### #登录

> mysql -u root -p

###### #查询服务状态

> mysqld

###### #查询数据库

> show databases;

###### #查询字符集

> #mysql数据库服务器字符集
>
> show variables like '%character%';
>
> show variables like 'collation%';
>
> #mysql支持的字符集
>
> show charset;
>
> #指定数据库的字符集
>
> show create database dbA\G;
>
> ​	#: dbA为数据库名；
>
> #指定表的字符集
>
> show table status from dbA like 'tbA';
>
> ​	#: dbA为数据库名；tbA为表名；
>
> #表中所有列的字符集
>
> show full columns from dbA;
>
> ​	#: tbA为表名；
>
> #创建数据库时指定字符集
>
> create database dbA default character set=utf8mb4;
>
> ​	#: dbA为数据库名；utf8mb4为字符集名；
>
> #修改数据库的字符集
>
> alter database dbA default character set uft8mb4;
>
> ​	#: tbA(id...为表信息；utf8mb4为字符集名；
>
> #创建表时指定字符集
>
> create table tbA(id int(6),name char(10)) default character set='utf8mb4';
>
> ​	#: tbA(id...为表信息；utf8mb4为字符集名；
>
> #修改表的字符集
>
> alter table tbA convert to character set uft8mb4;
>
> ​	#: tbA为表名；utf8mb4为字符集名；
>
> #修改表中指定列的字符集
>
> alter table tbA modify name char(10) character set uft8mb4;
>
> ​	#: tbA为表名；name char(10) 为列信息；utf8mb4为字符集名；
>
> #建立连接使用的编码
>
> set character_set_connection=utf8mb4;
>
> ​		#: utf8mb4为字符集名；
>
> #数据库服务器编码
>
> set character_set_server=utf8mb4;
>
> ​		#: utf8mb4为字符集名；
>
> #系统编码
>
> set character_set_system=utf8mb4;
>
> ​		#: utf8mb4为字符集名；
>
> #数据库编码
>
> set character_set_database=utf8mb4;
>
> ​		#: utf8mb4为字符集名；
>
> #结果集编码
>
> set character_set_results=utf8mb4;
>
> ​		#: utf8mb4为字符集名；
>
> #其他编码
>
> set collation_connection=utf8mb4;
>
> set collation_database=utf8mb4;
>
> set collation_server=utf8mb4;



###### #查询用户

> select user from mysql.user;

###### #新增用户

> create user user_new@localhost identified by '123456';
>
> #user_new为用户名； 123456为密码；

###### #用户授权

> #指定权限
>
> grant insert,update,delete,select privileges on dbA.\* to user_new@'%';
>
> ​	#：insert,update,delete,select为指定权限，all为全部权限；dbA为数据库名，*为所有数据库；user_new为用户名；123456为密码；%为访问地址；
>
> #刷新权限
>
> flush privileges;

###### #删除用户

> delete from mysql.user where user='user_new' and host='%';
>
> ​	#：user_new为用户名;%为访问地址；

###### #修改密码

> update mysql.user set password=password('123456') where user='user_new';
>
> ​	#：user_new为用户名；123456为新密码；

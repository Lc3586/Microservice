### #常用命令



## 资源

###### #查询资源使用情况

> top

###### #查询当前路径

> pwd

###### #查找文件"nginx"

> whereis nginx

###### #目录列表

> ls

###### #目录详情（包括权限信息）

> ll



## 权限

###### #新增用户组“admin”

> groupadd admin

###### #用户“root”添加至用户组

> usermod -a -G admin root

###### #当前文件夹递归授权给用户组

> chgrp admin -R ./

###### #设置用户组读/写/执行权限

> chmod g=rwx -R ./

###### #新增用户组写入权限

> chmod g+w -R ./

###### #删除用户组写入权限

> chmod g-w -R ./

###### #设置用户读/写/执行权限

> chmod u=rwx -R ./

###### #新增用户写入权限

> chmod u+w -R ./

###### #删除用户写入权限

> chmod u-w -R ./

###### #设置其他用户读/写/执行权限

> chmod o=rwx -R ./

###### #新增其他用户写入权限

> chmod o+w -R ./

###### #删除其他用户写入权限

> chmod o-w -R ./

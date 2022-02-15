# 在Docker中部署

> docker run -v app_storage_area:/rabbitmq-data -dit --name myrabbitmq -e RABBITMQ_DEFAULT_USER=admin -e RABBITMQ_DEFAULT_PASS="123456" -p 15672:15672 -p 5672:5672 rabbitmq:management

  * app_storage_area 挂载指定的存储卷
  * -e 设置环境变量
    * RABBITMQ_DEFAULT_USER 用户名
    * RABBITMQ_DEFAULT_PASS 密码
  * 15672 管理页面端口号
  * 5672 接口端口号

# 在Windows中安装（PowerShell）
> choco install rabbitmq
  * 如果提示找不到choco命令，可执行下面这句
    > iwr https://chocolatey.org/install.ps1 -UseBasicParsing | iex

# 用户列表
> rabbitmqctl list_user

# 添加用户
> rabbitmqctl add_user admin 123456
  * admin 用户名
  * 123456 密码

# 修改密码
> rabbitmqctl change_password admin 123456
  * admin 用户名
  * 123456 密码

# 清除密码
> rabbitmqctl clear_password admin
  * admin 用户名

# 设置角色（tags）
> rabbitmqctl set_user_tags admin administrator
  * admin 用户名
  * administrator 表示管理员角色，如果为空，则相当于删除所有角色

# 权限列表
> rabbitmqctl list_permissions

# 设置权限
> rabbitmqctl set_permissions -p / admin ".*" ".*" ".*"
  * / 路径
  * admin 用户名
  * ".*" ".*" ".*" 依次表示 配置、写入、读取
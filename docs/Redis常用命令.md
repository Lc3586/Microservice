# 在Docker中部署

> docker run -v app_storage_area:/redis-data -p 6379:6379 --name redis -d redis redis-server --save 60 1 --loglevel warning --requirepass "123456"
    
  * app_storage_area 挂载指定的存储卷
  * loglevel 日志级别
  * requirepass 密码
  * 6379 端口号

# 进入redis目录并允许cli
> cd /usr/local/bin
> 
> redis-cli

# 登录
> auth "123456"

# 查看密码
> config get requirepass

# 设置密码
> config set requirepass "123456"
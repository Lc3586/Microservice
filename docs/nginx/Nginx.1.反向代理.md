### #反向代理配置

> ###### #转发配置, 例如**proxy_pass**（**代理**）指定为http://testa/, 则会将请求转发至http://127.0.0.1:5000, 此**过程只存在于服务器**上，对于**用户不可见**。
>
> upstream testa {
> 	server	127.0.0.1:5000;
> }
>
> ###### #可配置多个
>
> upstream testb {
> 	server	127.0.0.1:5002;
> }
>
> ###### #站点配置
>
> server{
>
> ###### 	#监听端口
>
> ​	listen					80;
>
> ###### 	#服务名，一般填写**域名**或**ip地址**，此值对应的是开放给用户的**访问地址**
>
> ​	server_name	  test.api.com;
>
> ###### 	#路径配置，配置uri为**/a**，**proxy_pass**（**代理**）为http://testa/，则用户请求http://test.api.com/a时，服务器会将请求转发至http://127.0.0.1:5000（后续的路径或是参数皆会转发）
>
> ​	location	/a {
> ​		proxy_pass	http://testa/;
> ​		index			   index.html index.htm;
> ​	}
>
> ###### 	#可配置多个
>
> ​	location /b {
> ​		proxy_pass	http://testb/;
> ​		index			   index.html index.htm;
> ​	}
> }



### #特别说明：

#和**ssl搭配使用**时，直接在**server现有配置**中添加**ssl相关配置**即可，并在监听端口后面添加**ssl**标识（中间有**空格**）


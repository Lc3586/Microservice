### #TLS配置

> ###### #站点配置
>
> server{
>
> ###### 	#监听端口号+**ssl**，中间使用空格分割
>
> ​	listen					443 **ssl**;
>
> ###### 	#服务名，一般填写**域名**或**ip地址**，此值对应的是开放给用户的**访问地址**
>
> ​	server_name	  test.api.com;
>
> ###### 	#证书文件，只**包含公钥**的**pem文件**和**cer文件**皆可
>
> ​	ssl_certificate      "D:\WebSite\Certificate\\test.api.com.cer";
>
> ###### 	#私钥文件， **key文件**和**pvk文件**皆可
>
> ​	ssl_certificate_key  "D:\WebSite\Certificate\\test.api.com.pvk";
>
> ###### 	#会话超时时间
>
> ​	ssl_session_timeout 5m;
>
> ###### 	#支持的加密套件
>
> ​	ssl_ciphers ECDHE-RSA-AES128-GCM-SHA256:ECDHE:ECDH:AES:HIGH:!NULL:!aNULL:!MD5:!ADH:!RC4; 
>
> ###### 	#支持的协议
>
> ​	ssl_protocols TLSv1 TLSv1.1 TLSv1.2; 
>
> ###### 	#兼容**旧版协议**时使用此参数
>
> ​	ssl_prefer_server_ciphers on; 
>
> ​	location	/ {
> ​		root   html;
> ​		index			   index.html index.htm;
> ​	}
> }

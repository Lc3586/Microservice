### #目录配置

> ###### #站点配置
>
> server{
> 
> ###### 	#设置根目录
>
> root D:/WebSite/admin/;
>
> ###### 	#默认根目录为上面的配置
> 
> ​	location	/	{
>​		index			   index.html index.htm;
> ​	}
>
> ###### 	#更改特定路径的根目录（方式一）
>
> ​	location	/web/	{
>​		root	D:/WebSite/web/;
> ​		index			   index.html index.htm;
>​	}
> 
>###### 	#更改特定路径的根目录（方式二）
> 
>​	location	/web/	{
> ​		alias	D:/WebSite/web/;
>​		index			   index.html index.htm;
> ​	}
> 
> }


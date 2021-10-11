### #创建CA证书配置文件

> [ req ]
>
> #根证书名称
>
> distinguished_name  = localhost_ca
>
> x509_extensions     = root_ca
>
> 
>
> #和上方名称保持一致
>
> [ localhost_ca ]
>
> ##### #以下内容根据实际情况填写
>
> #国家代码
>
> countryName             = CN
>
> countryName_min         = 2
> countryName_max         = 2
>
> #省
>
> stateOrProvinceName     = ZheJiang
>
> #市
>
> localityName            = NingBo
>
> organizationName      = 
>
> #组织
>
> organizationalUnitName  = technology 
>
> commonName              =  
> commonName_max          = 
> emailAddress            =  
> emailAddress_max        = 
>
> [ root_ca ]
> basicConstraints            = critical, CA:true



### #创建SSL证书拓展配置文件

> subjectAltName = @alt_names
> extendedKeyUsage = serverAuth
>
> [alt_names]
>
> #域名，如有多个用DNS.2,DNS.3…来增加
>
> DNS.1 = localhost
> DNS.2 = test.api.com
>
> #IP地址
>
> IP.1 = 127.0.0.1
> IP.2 = 192.168.1.2



### #生成CA证书文件 cer（证书文件） pvk（私钥文件）

> openssl req -x509 -newkey rsa:2048 **-out** localhostCA.cer -outform PEM **-keyout** localhostCA.pvk **-days** 10000 -verbose **-config** localhostCA.cnf -nodes -sha256 **-subj** "/CN=localhost CA"

- **-out** 输出的证书文件
- **-keyout** 输出的私钥文件
- **-days** 使用期限
- **-config** 使用的CA证书配置文件
- **-subj** 主体信息



### #生成SSL证书文件 pvk（私钥文件） req（证书请求文件）

> openssl req -newkey rsa:2048 **-out** "localhost.req" **-keyout** "localhost.pvk" **-subj** "/CN=localhost" -sha256 -nodes

- **-out** 输出的证书请求文件
- **-keyout** 输出的私钥文件
- **-subj** 主体信息（一般填写绑定的域名或是ip地址，不包括端口号）



### #生成SSL证书文件 cer（证书文件）

> openssl x509 -req **-CA** localhostCA.cer **-CAkey** localhostCA.pvk **-in** localhost.req **-out** localhost.cer **-days** 10000 -extfile localhost.ext -sha256 -set_serial 0x1111

- **-CA** 根证书文件
- **-CAKey** 根证书密钥文件
- **-in** 输入的证书请求文件
- **-out** 输出的证书文件
- **-days** 使用期限
- **-extfile** 使用的SSL证书拓展配置文件



### #格式转换

##### #将cer（证书文件）和pvk（私钥文件）转换为pfx（同时保护公钥和私钥的证书文件），IIS应用服务器需要使用此文件

> openssl pkcs12 -export **-in** localhost.cer **-out** localhost.pfx **-inkey** localhost.pvk

- **-in** 输入的证书文件
- **-out** 输出的证书文件
- **-inkey** 证书私钥文件



##### #将crt（证书文件）转为cer（证书文件）

> openssl x509 -inform pem **-in** localhost.crt -outform PEM **-out** localhost.cer

- **-in** 输入的证书文件
- **-out** 输出的证书文件



##### #将cer（证书文件）转为pem（只包含公钥的证书文件）

> openssl x509 -inform PEM **-in** localhost.cer **-out** localhost.pem

- **-in** 输入的证书文件
- **-out** 输出的证书文件


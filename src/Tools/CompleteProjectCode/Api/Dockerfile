#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base

#��װffmpeg
RUN sed -i 's/deb.debian.org/mirrors.ustc.edu.cn/g' /etc/apt/sources.list \
    && apt-get update \
    && apt-get install -y ffmpeg \
    && apt-get clean && apt-get autoclean && apt-get autoremove \
    && rm -rf /var/lib/apt/lists/*

#��װlibgdiplus��
RUN apt-get update && apt-get install -y libgdiplus libc6-dev && ln -s /usr/lib/libgdiplus.so /usr/lib/gdiplus.dll

#��װlibncurses��
RUN apt-get install -y libncurses5

#����X-Server (xvfb)
ENV DISPLAY :10
COPY linux/xvfb.init /etc/init.d/xvfb
# Removes any windows line endings because linux can`t start the service in this case
RUN sed -i -e 's/\r//g' /etc/init.d/xvfb
RUN chmod +x /etc/init.d/xvfb
RUN update-rc.d xvfb defaults
# In the end we need to start the xserver and our API, so run.sh starts them in the right order

#����ʱ��Ϊ�й��Ϻ�
#��ʽһ
ENV TZ=Asia/Shanghai
ENV DEBIAN_FRONTEND noninteractive
# ��װ tzdata ������
RUN apt-get update \
    && apt-get install -y tzdata \
    && ln -fs /usr/share/zoneinfo/$TZ  /etc/localtime \
    && rm -rf /var/lib/apt/lists/ \
    && dpkg-reconfigure -f noninteractive tzdata
    
#��ʽ��
#RUN cp /usr/share/zoneinfo/Asia/Shanghai /etc/localtime && echo 'Asia/Shanghai' >/etc/timezone

WORKDIR /app
EXPOSE 80
EXPOSE 443

#����
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Applications/SimpleApi/Api/Api.csproj", "Applications/SimpleApi/Api/"]
COPY ["Applications/SimpleApi/IocServiceDemo/IocServiceDemo.csproj", "Applications/SimpleApi/IocServiceDemo/"]
COPY ["Applications/SimpleApi/SoapServices/SoapServices.csproj", "Applications/SimpleApi/SoapServices/"]
COPY ["Applications/SimpleApi/Model/Model.csproj", "Applications/SimpleApi/Model/"]
COPY ["Applications/SimpleApi/Entity/Entity.csproj", "Applications/SimpleApi/Entity/"]
COPY ["Applications/SimpleApi/Business/Business.csproj", "Applications/SimpleApi/Business/"]
RUN dotnet restore "Applications/SimpleApi/Api/Api.csproj"
COPY . .
WORKDIR "/src/Applications/SimpleApi/Api"
RUN dotnet build "Api.csproj" -c Release -o /app/build

#����
FROM build AS publish
RUN dotnet publish "Api.csproj" -c Release -o /app/publish

#����
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

#����֤��
COPY certificate/simple-api.crt /usr/local/share/ca-certificates/simple-api.crt
RUN update-ca-certificates

ADD linux/run.sh /run.sh
RUN chmod a+x /run.sh
CMD /run.sh

#ENTRYPOINT ["dotnet", "SimpleApi.dll"]

#���SQL SERVER���ݿ�TLS�汾�ϵ�
#����ѡ��װ������(����ʹ�ô˷�ʽ)
#���ݰ�װ��SQL�汾��˳��װ������
#�����г���2014�汾�����������ص�ַ
#Microsoft? SQL Server? 2014 Service Pack 1 (SP1)
#https://www.microsoft.com/zh-CN/download/details.aspx?id=46694
#Microsoft? SQL Server? 2014 Service Pack 2 (SP2)
#https://www.microsoft.com/zh-CN/download/details.aspx?id=53168
#Microsoft? SQL Server? 2014 Service Pack 3 (SP3)
#https://www.microsoft.com/zh-CN/download/details.aspx?id=57474
#SQL Server 2014 SP3 ��ȫ���³��� (KB4505218)
#https://www.microsoft.com/zh-cn/download/details.aspx?id=58487
#���߽���TLS�汾
#��ʽһ
#RUN sed -i 's/TLSv1.2/TLSv1.0/g' /etc/ssl/openssl.cnf
#��ʽ��
#RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /etc/ssl/openssl.cnf
#RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /usr/lib/ssl/openssl.cnf

#������
#docker volume create --name app_storage_area --label "��ע"="Ӧ�ó��򹫹��洢Ŀ¼��"

#��������
#docker build -f Dockerfile.published -t lctr/simple-api .

#��������������
#docker run -v app_storage_area:/app-data --name simple-api -p 5001:443 -p 5000:80 --name simple-api --restart always lctr/simple-api

#��������
#docker exec -it [id/name] bash

#Ŀǰ������Զ�ת��wwwroot�е��ļ�,�����ֶ�����
#�������ݵ���������
#cp -r /app/wwwroot /app-data/simple-api
#����Ŀ¼
#rm -rf /app/wwwroot
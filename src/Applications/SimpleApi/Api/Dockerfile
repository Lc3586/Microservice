#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
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

FROM build AS publish
RUN dotnet publish "Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimpleApi.dll"]

#构建镜像
#DOCKER build -t lctr/simple-api .
#创建卷
#DOCKER volume create --name simple-api-file_storage
#创建容器并运行
#DOCKER run -it --rm -v simple-api-file_storage lctr/simple-api
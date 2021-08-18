using Microservice.Library.Cache.Model;
using Microservice.Library.Configuration.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Model.Utils.Config
{
    /// <summary>
    /// 系统配置
    /// </summary>
    public class SystemConfig
    {
        #region 基础

        /// <summary>
        /// 当前操作系统平台
        /// </summary>
        public string CurrentOS
        {
            get
            {
                if (_CurrentOS != null)
                    return _CurrentOS;

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    _CurrentOS = OSPlatform.Windows.ToString();
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    _CurrentOS = OSPlatform.Linux.ToString();
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    _CurrentOS = OSPlatform.OSX.ToString();
                else
                    throw new ApplicationException("无法获取当前的操作系统平台.");

                return _CurrentOS;
            }
        }

        /// <summary>
        /// 当前操作系统平台
        /// </summary>
        private string _CurrentOS { get; set; }

        /// <summary>
        /// 数据存储根目录
        /// </summary>
        /// <remarks>
        /// <para>将会更改<see cref="Microsoft.AspNetCore.Hosting.IHostingEnvironment.ContentRootPath"/>的值</para>
        /// <para>用于存放应用程序运行过程中需要持久化的文件数据</para>
        /// <para>指定此目录时，将会从中获取wwwroot目录</para>
        /// <para>不指定时默认使用程序根目录</para>
        /// </remarks>
        public string StorageDirectory { get; set; }

        /// <summary>
        /// 数据存储根目录绝对路径
        /// </summary>
        public string AbsoluteStorageDirectory
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_AbsoluteStorageDirectory))
                    _AbsoluteStorageDirectory = Path.GetFullPath(StorageDirectory ?? string.Empty, AppContext.BaseDirectory);
                if (!Directory.Exists(_AbsoluteStorageDirectory))
                    Directory.CreateDirectory(_AbsoluteStorageDirectory);
                return _AbsoluteStorageDirectory;
            }
            set
            {
                _AbsoluteStorageDirectory = value;
            }
        }

        /// <summary>
        /// 数据存储根目录绝对路径
        /// </summary>
        private string _AbsoluteStorageDirectory { get; set; }

        /// <summary>
        /// 站点资源文件根目录绝对路径
        /// </summary>
        public string AbsoluteWWWRootDirectory
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_AbsoluteWWWRootDirectory))
                    _AbsoluteWWWRootDirectory = Path.Combine(AbsoluteStorageDirectory, "wwwroot");
                if (!Directory.Exists(_AbsoluteWWWRootDirectory))
                    Directory.CreateDirectory(_AbsoluteWWWRootDirectory);
                return _AbsoluteWWWRootDirectory;
            }
            set
            {
                _AbsoluteWWWRootDirectory = value;
            }
        }

        /// <summary>
        /// 站点资源文件根目录绝对路径
        /// </summary>
        private string _AbsoluteWWWRootDirectory { get; set; }

        /// <summary>
        /// 应用程序保护数据存储目录
        /// </summary>
        public string DataProtectionDirectory { get; set; }

        /// <summary>
        /// 应用程序保护数据存储目录绝对路径
        /// </summary>
        public string AbsoluteDataProtectionDirectory
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_AbsoluteDataProtectionDirectory))
                    _AbsoluteDataProtectionDirectory = Path.GetFullPath(DataProtectionDirectory ?? string.Empty, AppContext.BaseDirectory);
                if (!Directory.Exists(_AbsoluteDataProtectionDirectory))
                    Directory.CreateDirectory(_AbsoluteDataProtectionDirectory);
                return _AbsoluteDataProtectionDirectory;
            }
            set
            {
                _AbsoluteDataProtectionDirectory = value;
            }
        }

        /// <summary>
        /// 应用程序保护数据存储目录绝对路径
        /// </summary>
        private string _AbsoluteDataProtectionDirectory { get; set; }

        /// <summary>
        /// 用于加密应用程序保护数据的秘钥文件(.pfx)
        /// </summary>
        public string DataProtectionCertificateFile { get; set; }

        /// <summary>
        /// 用于加密应用程序保护数据的秘钥文件(.pfx)绝对路径
        /// </summary>
        public string AbsoluteDataProtectionCertificateFile
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(DataProtectionCertificateFile) && string.IsNullOrWhiteSpace(_AbsoluteDataProtectionCertificateFile))
                    _AbsoluteDataProtectionCertificateFile = Path.GetFullPath(DataProtectionCertificateFile, AppContext.BaseDirectory);
                return _AbsoluteDataProtectionCertificateFile;
            }
        }

        /// <summary>
        /// 用于加密应用程序保护数据的秘钥文件(.pfx)绝对路径
        /// </summary>
        private string _AbsoluteDataProtectionCertificateFile { get; set; }

        /// <summary>
        /// 用于加密应用程序保护数据的秘钥文件(.pfx)的密码
        /// </summary>
        public string DataProtectionCertificateFilePassword { get; set; }

        /// <summary>
        /// 超级管理员Id
        /// </summary>
        public string AdminId { get; set; }

        /// <summary>
        /// 超级管理员账号
        /// </summary>
        public string AdminAccount { get; set; }

        /// <summary>
        /// 超级管理员账号初始化密码
        /// </summary>
        public string AdminInitPassword { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// 网站根地址
        /// </summary>
        public string WebRootUrl
        {
            get
            {
                if (RunMode == RunMode.Publish || RunMode == RunMode.Publish_Swagger)
                    return PublishRootUrl;
                else
                    return LocalRootUrl;
            }
        }

        /// <summary>
        /// 发布后网站根地址
        /// </summary>
        public string PublishRootUrl { get; set; }

        /// <summary>
        /// 本地调试根地址
        /// </summary>
        public string LocalRootUrl { get; set; }

        /// <summary>
        /// 运行模式
        /// </summary>
        public RunMode RunMode { get; set; }

        /// <summary>
        /// 服务器标识
        /// </summary>
        public string ServerKey { get; set; }

        /// <summary>
        /// Docker桥接网络模式
        /// </summary>
        public bool DockerBridge { get; set; }

        #endregion

        #region 高级

        /// <summary>
        /// 标记了IDependency的类的命名空间集合
        /// <para>支持通配符</para>
        /// </summary>
        public List<string> FxAssembly { get; set; }

        #endregion

        #region 业务

        /// <summary>
        /// 数据删除模式,默认逻辑删除
        /// </summary>
        public DeleteMode DeleteMode { get; set; }

        /// <summary>
        /// 工作ID
        /// </summary>
        public long WorkerId { get; set; }

        /// <summary>
        /// 简易身份认证
        /// </summary>
        public bool EnableSampleAuthentication { get; set; }

        /// <summary>
        /// 启用站点
        /// </summary>
        public List<string> EnableSite { get; set; }

        #endregion

        #region 日志

        /// <summary>
        /// 默认日志组件名称
        /// </summary>
        public string DefaultLoggerName { get; set; } = "SystemLog";

        /// <summary>
        /// 默认日志组件类型
        /// </summary>
        public LoggerType DefaultLoggerType { get; set; } = LoggerType.Console;

        /// <summary>
        /// 默认日志组件布局
        /// </summary>
        public string DefaultLoggerLayout { get; set; }

        /// <summary>
        /// 需要记录的日志的最低等级
        /// </summary>
        public int MinLogLevel { get; set; } = 3;//LogLevel.Warning;

        #endregion

        #region 自动生成代码应用程序

        /// <summary>
        /// 启用自动生成代码应用程序
        /// </summary>
        public bool EnableCAGC { get; set; }

        #endregion

        #region Swagger

        /// <summary>
        /// 启用Swagger
        /// </summary>

        public bool EnableSwagger { get; set; }

        /// <summary>
        /// Swagger配置
        /// </summary>
        [JsonConfig("jsonconfig/swagger.json")]
        public SwaggerSetting Swagger { get; set; }

        #endregion

        #region 缓存

        /// <summary>
        /// 启用缓存
        /// </summary>
        public bool EnableCache { get; set; }

        /// <summary>
        /// 默认缓存类型
        /// </summary>
        public CacheType DefaultCacheType { get; set; }

        /// <summary>
        /// Redis配置
        /// </summary>
        [JsonConfig("jsonconfig/redis.json", "FreeRedis")]
        public RedisSetting Redis { get; set; }

        #endregion

        #region 数据映射

        /// <summary>
        /// 启用AutoMapper
        /// </summary>
        public bool EnableAutoMapper { get; set; }

        /// <summary>
        /// AutoMapper命名空间集合
        /// <para>支持通配符</para>
        /// </summary>
        public List<string> AutoMapperAssemblys { get; set; }

        #endregion

        #region 数据库

        /// <summary>
        /// 启用多数据库
        /// </summary>
        public bool EnableMultiDatabases { get; set; }

        /// <summary>
        /// 单数据库配置
        /// </summary>
        [JsonConfig("jsonconfig/database.json", "Single")]
        public DatabaseSetting Database { get; set; }

        /// <summary>
        /// 多数据库配置
        /// </summary>
        [JsonConfig("jsonconfig/database.json", "Multi")]
        public List<DatabaseSetting> Databases { get; set; }

        #endregion

        #region FreeSql

        /// <summary>
        /// 启用FreeSql
        /// </summary>
        public bool EnableFreeSql { get; set; }

        /// <summary>
        /// FreeSql配置
        /// </summary>
        [JsonConfig("jsonconfig/freesql.json", "FreeSql")]
        public FreeSqlSetting FreeSql { get; set; }

        #endregion

        #region ES服务

        /// <summary>
        /// 启用ElasticSearch
        /// </summary>
        public bool EnableElasticsearch { get; set; }

        /// <summary>
        /// ElasticSearch配置
        /// </summary>
        [JsonConfig("jsonconfig/elasticsearch.json", "ElasticSearch")]
        public ElasticsearchSetting Elasticsearch { get; set; }

        #endregion

        #region Kafka中间件

        /// <summary>
        /// 启用Kafka中间件
        /// </summary>
        public bool EnableKafka { get; set; }

        /// <summary>
        /// kafka中间件配置
        /// </summary>
        [JsonConfig("jsonconfig/kafka.json", "Kafka")]
        public KafkaSetting Kafka { get; set; }

        #endregion

        #region Soap

        /// <summary>
        /// 启用Soap
        /// </summary>
        public bool EnableSoap { get; set; }

        /// <summary>
        /// Soap配置
        /// </summary>
        [JsonConfig("jsonconfig/soap.json", "Soaps")]
        public List<SoapSetting> Soaps { get; set; }

        #endregion

        #region CAS

        /// <summary>
        /// 启用CAS
        /// </summary>
        public bool EnableCAS { get; set; }

        /// <summary>
        /// CAS配置
        /// </summary>
        [JsonConfig("jsonconfig/cas.json", "CAS")]
        public CASSetting CAS { get; set; }

        #endregion

        #region JWT

        /// <summary>
        /// 启用JWT
        /// </summary>
        public bool EnableJWT { get; set; }

        /// <summary>
        /// JWT配置
        /// </summary>
        [JsonConfig("jsonconfig/jwt.json", "JWT")]
        public JWTSetting JWT { get; set; }

        #endregion

        #region 微信服务

        /// <summary>
        /// 启用微信服务
        /// </summary>
        public bool EnableWeChatService { get; set; }

        /// <summary>
        /// 微信服务配置
        /// </summary>
        [JsonConfig("jsonconfig/wechatservice.json", "WeChatService")]
        public WeChatServiceSetting WeChatService { get; set; }

        #endregion

        #region Signalr

        /// <summary>
        /// 启用Signalr服务
        /// </summary>
        public bool EnableSignalr { get; set; }

        /// <summary>
        /// Signalr允许跨域
        /// </summary>
        public bool SignalrCors { get; set; }

        #endregion

        #region 文件清单

        /// <summary>
        /// 文件清单
        /// <para>[{文件名 : [{平台 : 文件路径}]}]</para>
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        [JsonConfig("jsonconfig/filelist.json", "FileList")]
        public Dictionary<string, Dictionary<string, string>> FileList { get; set; }

        private Dictionary<string, Dictionary<string, string>> FileAbsolutePathList { get; set; } = new Dictionary<string, Dictionary<string, string>>();

        /// <summary>
        /// 获取文件清单中文件的绝对路径
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns></returns>
        public string GetFileAbsolutePath(string filename)
        {
            string result;
            if (FileAbsolutePathList.ContainsKey(filename) && FileAbsolutePathList[filename].ContainsKey(CurrentOS))
                result = FileAbsolutePathList[filename][CurrentOS];
            else
            {
                if (!FileList.ContainsKey(filename))
                    throw new ApplicationException("未在jsonconfig/filelist.json中配置{filename}文件");

                if (!FileList[filename].ContainsKey(CurrentOS))
                    throw new ApplicationException($"未找到符合当前系统【{CurrentOS}】版本的{filename}文件");

                result = Path.GetFullPath(FileList[filename][CurrentOS], AppContext.BaseDirectory);

                if (!FileAbsolutePathList.ContainsKey(filename))
                    FileAbsolutePathList.Add(filename, new Dictionary<string, string>());

                FileAbsolutePathList[filename].Add(CurrentOS, result);
            }

            if (!File.Exists(result))
                throw new ApplicationException($"指定目录下未找到{filename}文件: {result}.");

            return result;
        }

        #endregion

        #region RSA

        /// <summary>
        /// 启用RSA
        /// </summary>
        public bool EnableRSA { get; set; }

        /// <summary>
        /// RSA配置
        /// </summary>
        [JsonConfig("jsonconfig/rsa.json", "RSA")]
        public RSASetting RSA { get; set; }

        #endregion

        #region 大文件上传功能

        /// <summary>
        /// 启用大文件上传功能
        /// </summary>
        public bool EnableUploadLargeFile { get; set; }

        #endregion

        #region MyRegion

        /// <summary>
        /// 启用接口版本
        /// </summary>
        public bool EnableApiVersion { get; set; }

        /// <summary>
        /// 接口版本配置
        /// </summary>
        [JsonConfig("jsonconfig/apiversion.json", "ApiVersion")]
        public ApiVersionSetting ApiVersion { get; set; }

        #endregion
    }
}

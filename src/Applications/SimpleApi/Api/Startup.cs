using Api.Configures;
using Api.Middleware;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using Business.Handler;
using IocServiceDemo;
using Microservice.Library.Configuration;
using Microservice.Library.ConsoleTool;
using Microservice.Library.Container;
using Microservice.Library.Extension;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Model.Utils.Config;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Api
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            //if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            //    Bar = new ProgressBar();
            //else
            //    Bar = new ProgressBar(1, 50, ProgressBar.ProgressBarType.Character);

            Bar?.Normal(5, "正在读取配置.");
            Configuration = configuration;
            Config = new ConfigHelper(Configuration).GetModel<SystemConfig>("SystemConfig");
            "已读取配置.".ConsoleWrite();
            Bar?.Normal(6);
            Console.Title = Config.ProjectName;
            Config.ProjectName.ConsoleWrite();
            $"运行模式 => {Config.RunMode}.".ConsoleWrite();
        }

        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// 系统配置
        /// </summary>
        public SystemConfig Config { get; }

        private ProgressBar Bar { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <remarks>
        /// <para>This method gets called by the runtime. Use this method to add services to the container.</para>
        /// <para>For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940</para>
        /// </remarks>
        public void ConfigureServices(IServiceCollection services)
        {
            Bar?.Normal(6, "正在注入依赖.");
            services.RegisterDataProtection(Config);

            Bar?.Normal(7);
            #region 注册服务示例代码

            "注册示例服务.".ConsoleWrite();

            //注册成单例
            //.AddSingleton(typeof(IDemoService),typeof(DemoServiceA))

            //使用工厂模式注册服务
            services.AddScoped(typeof(IDemoService), DemoServiceProvider.GetService);

            //通过自定义方法注册服务构造器
            services.AddDemoService(options =>
            {
                options.Threshold = 2;
                //options.DisableType.Add(typeof(DemoServiceC));
            });

            #endregion

            Bar?.Normal(8);
            "注册控制器.".ConsoleWrite();
            "注册全局异常拦截器.".ConsoleWrite();
            var mvcbuilder = services.AddControllers(options =>
              {
                  options.Filters.Add<GlobalExceptionFilter>();
              });

            Bar?.Normal(9);
            "注册NewtonsoftJson.".ConsoleWrite();
            mvcbuilder.AddNewtonsoftJson(options =>
            {
                //可在此配置Json序列化全局设置
                //options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });

            Bar?.Normal(10);
            "注册控制器为服务.".ConsoleWrite();
            mvcbuilder.AddControllersAsServices();

            "注册HttpContext访问器IHttpContextAccessor, 生命周期 => Scoped.".ConsoleWrite();
            services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();

            "注册ActionContext访问器IActionContextAccessor, 生命周期 => Transient.".ConsoleWrite();
            services.AddTransient<IActionContextAccessor, ActionContextAccessor>();

            "注册Configuration, 生命周期 => Singleton.".ConsoleWrite();
            services.AddSingleton(Configuration);

            "注册系统配置, 生命周期 => Singleton.".ConsoleWrite();
            services.AddSingleton(Config);

            "注册日志服务.".ConsoleWrite();
            services.AddLogging();

            "注册Kestrel服务.".ConsoleWrite();
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            "注册IIS服务.".ConsoleWrite();
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            if (Config.EnableApiVersion)
                services.RegisterApiVersion(Config);

            Bar?.Normal(15);
            //不是发布模式时，开放swagger接口文档
            if (Config.EnableSwagger && Config.RunMode != RunMode.Publish)
            {
                if (Config.EnableApiVersion)
                    services.RegisterSwaggerMultiVersion(Config);
                else
                    services.RegisterSwagger(Config);
            }

            Bar?.Normal(16);
            if (Config.EnableCache)
                services.RegisterCache(Config);

            Bar?.Normal(17);
            if (Config.EnableSampleAuthentication)
                services.RegisterSampleAuthentication(Config);

            Bar?.Normal(18);
            if (Config.EnableCAS)
                services.RegisterCAS(Config);

            if (Config.EnableJWT)
                services.RegisterJWT(Config);

            Bar?.Normal(19);
            if (Config.EnableElasticsearch)
                services.RegisterElasticsearch(Config);

            Bar?.Normal(20);
            if (Config.EnableKafka)
                services.RegisterKafka(Config);

            Bar?.Normal(21);
            if (Config.EnableFreeSql)
            {
                if (Config.EnableMultiDatabases)
                    services.RegisterFreeSqlMultiDatabase(Config);
                else
                    services.RegisterFreeSql(Config);
            }

            Bar?.Normal(22);
            if (Config.EnableAutoMapper)
                services.RegisterAutoMapper(Config);

            Bar?.Normal(23);
            if (Config.EnableWeChatService)
                services.RegisterWeChat(Configuration, Config);

            Bar?.Normal(24);
            if (Config.EnableSoap)
                services.RegisterSoap(Config);

            Bar?.Normal(25);
            if (Config.EnableSignalr)
                services.RegisterSignalR(Config);

            Bar?.Normal(26);
            services.RegisterNLog(Config);

            Bar?.Normal(27);
            if (Config.EnableUploadLargeFile)
            {
                $"初始化{ChunkFileMergeHandler.Name}.".ConsoleWrite();
                services.AddSingleton(new ChunkFileMergeHandler());
            }

            Bar?.Normal(28);
            if (Config.EnableRSA)
                services.RegisterRSAHelper(Config);
        }

        /// <summary>
        /// 配置Autofac容器
        /// </summary>
        /// <param name="builder"></param>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            Bar?.Normal(58, "正在配置Autofac容器");

            "配置Autofac容器.".ConsoleWrite();

            "注册IDependency接口.".ConsoleWrite();

            // 在这里添加服务注册
            var baseType = typeof(IDependency);

            //自动注入IDependency接口,支持AOP,生命周期为InstancePerDependency
            var diTypes = Config.FxAssembly.GetTypes()
                .Where(x => baseType.IsAssignableFrom(x) && x != baseType)
                .ToArray();
            builder.RegisterTypes(diTypes)
                .AsImplementedInterfaces()
                .PropertiesAutowired()
                .InstancePerDependency()
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(Interceptor));

            Bar?.Normal(59);
            "注册控制器.".ConsoleWrite();
            //注册Controller
            builder.RegisterAssemblyTypes(typeof(Startup).GetTypeInfo().Assembly)
                .Where(t => typeof(Controller).IsAssignableFrom(t) && t.Name.EndsWith(nameof(Controller), StringComparison.Ordinal))
                .PropertiesAutowired();

            Bar?.Normal(50);
            "注册Interceptor(AOP).".ConsoleWrite();
            //AOP
            builder.RegisterType<Interceptor>();

            Bar?.Normal(51);
            "注册DisposableContainer(请求结束自动释放).".ConsoleWrite();
            //请求结束自动释放
            builder.RegisterType<DisposableContainer>()
                .As<IDisposableContainer>()
                .InstancePerLifetimeScope();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="hostingEnvironment"></param>
        /// <remarks>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </remarks>
#pragma warning disable CS0618 // 类型或成员已过时
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostingEnvironment hostingEnvironment)
#pragma warning restore CS0618 // 类型或成员已过时
        {
            "获取AutofacIOC容器.".ConsoleWrite();
            //获取AutofacIOC容器
            AutofacHelper.Container = app.ApplicationServices.GetAutofacRoot();

            Bar?.Normal(82, "正在配置应用程序");
            app.ConfiguraHostEnvironment(env, hostingEnvironment, Config);

            Bar?.Normal(83);
            "允许重复读取请求Body.".ConsoleWrite();
            //允许重复读取请求Body
            app.Use(next => context =>
            {
                context.Request.EnableBuffering();

                return next(context);
            });

            Bar?.Normal(84);
            "使用异常处理中间件.".ConsoleWrite();
            //处理异常
            app.UseMiddleware<ExceptionHandlerMiddleware>();

            Bar?.Normal(85);
            "使用跨域中间件.".ConsoleWrite();
            //跨域
            app.UseMiddleware<CorsMiddleware>();
            //app.UseDeveloperExceptionPage();

            Bar?.Normal(86);
            "使用静态文件服务.".ConsoleWrite();
            app.UseStaticFiles(new StaticFileOptions
            {
                //替换静态文件默认目录
                FileProvider = new PhysicalFileProvider(Config.AbsoluteWWWRootDirectory),
                ServeUnknownFileTypes = true,
                DefaultContentType = "application/octet-stream"
            });

            Bar?.Normal(87);
            "使用路由中间件.".ConsoleWrite();
            app.UseRouting();

            Bar?.Normal(88);
            //不是发布模式时，开放swagger接口文档
            if (Config.EnableSwagger && Config.RunMode != RunMode.Publish)
            {
                if (Config.EnableApiVersion)
                    app.ConfiguraSwaggerMultiVersion(Config);
                else
                    app.ConfiguraSwagger(Config);
            }

            Bar?.Normal(89);
            if (Config.EnableSampleAuthentication)
                app.ConfiguraSampleAuthentication(Config);

            if (Config.EnableJWT)
                app.ConfiguraJWT(Config);

            Bar?.Normal(80);
            if (Config.EnableCAS)
                app.ConfiguraCAS(Config);

            Bar?.Normal(91);
            if (Config.EnableFreeSql)
            {
                if (Config.EnableMultiDatabases)
                    app.ConfiguraFreeSqlMultiDatabase(Config);
                else
                    app.ConfiguraFreeSql(Config);
            }

            Bar?.Normal(92);
            if (Config.EnableWeChatService)
                app.ConfiguraWeChat(Config);

            Bar?.Normal(93);
            if (Config.EnableKafka)
                app.ConfiguraKafka(Config);

            Bar?.Normal(94);
            "使用终端中间件.".ConsoleWrite();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            Bar?.Normal(95);
            $"使用反向代理 {(Config.DockerBridge ? "Docker桥接网络模式" : "")}.".ConsoleWrite();
            //Nginx服务器
            app.UseForwardedHeaders(Config.DockerBridge
                ? new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.All,
                    KnownNetworks = { new IPNetwork(IPAddress.Parse("172.17.0.0"), 16) }
                }
                : new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                });

            Bar?.Normal(96);
            if (Config.EnableSoap)
                app.ConfiguraSoap(Config);

            Bar?.Normal(97);
            if (Config.EnableSignalr)
                app.ConfiguraSignalR(Config);

            Bar?.Normal(98);

            Bar?.Normal(99);
            if (Config.EnableUploadLargeFile)
            {
                $"启动{ChunkFileMergeHandler.Name}.".ConsoleWrite();
                AutofacHelper.GetService<ChunkFileMergeHandler>().Start();
            }

            Bar?.Normal(100, "应用程序已启动");
            "应用程序已启动.".ConsoleWrite(ConsoleColor.White, null, true, 1);
        }
    }
}

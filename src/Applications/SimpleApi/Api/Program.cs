using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Text;
using System.Linq;

namespace Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var chartset = args?.FirstOrDefault(o => o.IndexOf("-Chartset") == 0);
            if (chartset != default)
            {
                chartset = chartset.Replace("-Chartset", "").Trim();
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                Console.OutputEncoding = Encoding.GetEncoding(chartset);
            }
            Console.WriteLine("\r\n正在启动应用程序.");
            Console.WriteLine($"{TimeZoneInfo.Local.DisplayName} {DateTime.Now}");
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                //使用Autofac替换原有IOC容器
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureAppConfiguration((builder, config) =>
                    {
                        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                              .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true);
                    })
                    .UseStartup<Startup>();
                });
    }
}

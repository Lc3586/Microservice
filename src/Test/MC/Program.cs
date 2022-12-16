﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MC
{
    /// <summary>
    /// MVC客户端身份验证服务使用示例程序
    /// <para>LCTR 2019-05-29</para>
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "Mvc Client";
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}

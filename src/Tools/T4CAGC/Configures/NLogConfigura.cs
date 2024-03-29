﻿using Microservice.Library.NLogger.Application;
using Microsoft.Extensions.DependencyInjection;
using NLog.Targets;
using System.IO;
using System.Text;
using T4CAGC.Log;

namespace T4CAGC
{
    /// <summary>
    /// NLog配置类
    /// </summary>
    public static class NLogConfigura
    {
        /// <summary>
        /// 注册NLog服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="type">日志组件类型</param>
        /// <param name="minLogLevel">需要记录的日志的最低等级</param>
        public static IServiceCollection RegisterNLog(this IServiceCollection services, LoggerType type, int minLogLevel)
        {
            services.AddNLogger(s =>
            {
                switch (type)
                {
                    case LoggerType.File:
                        s.TargetGeneratorOptions
                            .Add(new TargetGeneratorOptions
                            {
                                MinLevel = NLog.LogLevel.FromOrdinal(minLogLevel),
                                Target = new FileTarget
                                {
                                    Name = LoggerConfig.LogName,
                                    Layout = LoggerConfig.Layout,
                                    FileName = Path.Combine(Directory.GetCurrentDirectory(), LoggerConfig.FileDic, LoggerConfig.FileName),
                                    Encoding = Encoding.UTF8
                                }
                            });
                        break;
                    case LoggerType.Console:
                    default:
                        if (minLogLevel < NLog.LogLevel.Error.Ordinal)
                        {
                            s.TargetGeneratorOptions
                                .Add(new TargetGeneratorOptions
                                {
                                    MinLevel = NLog.LogLevel.FromOrdinal(minLogLevel),
                                    MaxLevel = NLog.LogLevel.Warn,
                                    Target = new ColoredConsoleTarget
                                    {
                                        Name = LoggerConfig.LogName,
                                        Layout = LoggerConfig.Layout
                                    }
                                });
                        }

                        s.TargetGeneratorOptions
                            .Add(new TargetGeneratorOptions
                            {
                                MinLevel = NLog.LogLevel.Error,
                                Target = new ColoredConsoleTarget
                                {
                                    Name = LoggerConfig.LogName,
                                    Layout = LoggerConfig.Layout,
                                    ErrorStream = true
                                }
                            });
                        break;
                }
            })
            .AddMSLogger();

            return services;
        }
    }
}

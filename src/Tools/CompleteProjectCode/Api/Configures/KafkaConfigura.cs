using Business.Utils.Kafka;
using Business.Utils.Log;
using Confluent.Kafka;
using Microservice.Library.ConsoleTool;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Model.Utils.Config;
using Model.Utils.Log;

namespace Api.Configures
{
    /// <summary>
    /// Kafka服务配置类
    /// </summary>
    public static class KafkaConfigura
    {
        /// <summary>
        /// 注册Kafka服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        public static IServiceCollection RegisterKafka(this IServiceCollection services, SystemConfig config)
        {
            "注册Kafka服务.".ConsoleWrite();

            if (config.Kafka.EnableConsume)
                services.AddSingleton(new KafkaConsume<string, string>(new ConsumerConfig
                {
                    GroupId = config.Kafka.ConsumeGroupId,
                    BootstrapServers = config.Kafka.ConsumeServers,
                    AutoOffsetReset = config.Kafka.ConsumeReset
                }));

            if (config.Kafka.EnableProducer)
                services.AddSingleton(new KafkaProducer<string, string>(new ProducerConfig
                {
                    BootstrapServers = config.Kafka.ProducerServers
                }));

            return services;
        }

        /// <summary>
        /// 配置Kafka服务
        /// </summary>
        /// <param name="app"></param>
        /// <param name="config"></param>
        public static IApplicationBuilder ConfiguraKafka(this IApplicationBuilder app, SystemConfig config)
        {
            "配置Kafka服务.".ConsoleWrite();

            if (config.Kafka.EnableConsume)
                app.ApplicationServices
                    .GetService<KafkaConsume<string, string>>()
                    .Subscribe(config.Kafka.ConsumeTopics.ToArray())
                    .ReceiveMessage = (topic, message) =>
                    {
                        Logger.Log(
                            NLog.LogLevel.Trace,
                            LogType.系统跟踪,
                            $"kafka topic: {topic}, receive message, key: {message.Key}.",
                            message.Value.ToString());
                    };

            return app;
        }
    }
}

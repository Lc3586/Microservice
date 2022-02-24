using DataMigration.Application.Log;
using DataMigration.Application.Model;
using System;
using System.Threading.Tasks;

namespace DataMigration.Application.Handler
{
    /// <summary>
    /// 数据迁移处理器
    /// </summary>
    public class DataMigrationHandler : IHandler
    {
        public DataMigrationHandler(
            Config config,
            EntityHandler entityHandler,
            SchemeHandler schemeHandler,
            DataHandler dataHandler)
        {
            Config = config;
            EntityHandler = entityHandler;
            SchemeHandler = schemeHandler;
            DataHandler = dataHandler;
        }

        /// <summary>
        /// 配置
        /// </summary>
        readonly Config Config;

        /// <summary>
        /// 实体类处理器
        /// </summary>
        readonly EntityHandler EntityHandler;

        /// <summary>
        /// 模式处理器
        /// </summary>
        readonly SchemeHandler SchemeHandler;

        /// <summary>
        /// 数据处理器
        /// </summary>
        readonly DataHandler DataHandler;

        /// <summary>
        /// 处理
        /// </summary>
        public async Task Handler()
        {
            await Task.Run(EntityHandler.Handler);

            switch (Config.OperationType)
            {
                case OperationType.All:
                    await Task.Run(SchemeHandler.Handler);
                    await Task.Run(DataHandler.Handler);
                    break;
                case OperationType.Schema:
                    await Task.Run(SchemeHandler.Handler);
                    break;
                case OperationType.Data:
                    await Task.Run(DataHandler.Handler);
                    break;
                default:
                    throw new ApplicationException($"不支持的操作类型 {Config.OperationType}");
            }

            Logger.Log(NLog.LogLevel.Info, LogType.系统信息, "处理结束.");
        }
    }
}

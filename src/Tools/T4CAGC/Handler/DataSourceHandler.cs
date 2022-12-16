using Microservice.Library.Extension;
using System;
using T4CAGC.Log;
using T4CAGC.Model;

namespace T4CAGC.Handler
{
    /// <summary>
    /// 数据源处理类
    /// </summary>
    public class DataSourceHandler : IHandler
    {
        public DataSourceHandler(
            Config config,
            CsvFileHandler csvFileHandler,
            DatabaseHandler databaseHandler)
        {
            Config = config;
            CsvFileHandler = csvFileHandler;
            DatabaseHandler = databaseHandler;
        }

        /// <summary>
        /// 配置
        /// </summary>
        readonly Config Config;

        /// <summary>
        /// CSV文件处理器
        /// </summary>
        readonly CsvFileHandler CsvFileHandler;

        /// <summary>
        /// 数据库处理器
        /// </summary>
        readonly DatabaseHandler DatabaseHandler;

        /// <summary>
        /// 处理
        /// </summary>
        /// <returns>数据表信息</returns>
        public void Handler()
        {
            var tables = Config.DataSourceType switch
            {
                DataSourceType.CSV => CsvFileHandler.Handler(false),
                DataSourceType.CSV_Simple => CsvFileHandler.Handler(true),
                DataSourceType.DataBase => DatabaseHandler.Handler(),
                _ => throw new ApplicationException($"不支持的数据源类型 {Config.DataSourceType}")
            };

            if (!tables.Any_Ex())
                throw new ApplicationException("未获取到任何数据表.");

            tables.ForEach(o => Logger.Log(NLog.LogLevel.Info, LogType.系统信息, $"已获取数据表: {o.Name}{(o.FreeSql ? " [FreeSql]" : "")}{(o.Elasticsearch ? " [Elasticsearch]" : "")}{(o.Tree ? " [Tree]" : "")}."));

            Extension.Extension.SetTableInfos(tables);
        }
    }
}

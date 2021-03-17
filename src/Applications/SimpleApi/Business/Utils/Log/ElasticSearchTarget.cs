using Business.Hub;
using Entity.System;
using Microservice.Library.Container;
using Microservice.Library.Elasticsearch;
using Microservice.Library.Elasticsearch.Gen;
using Microsoft.AspNetCore.SignalR;
using Model.Utils.Config;
using Model.Utils.SignalR;
using NLog;
using NLog.Targets;

namespace Business.Utils.Log
{
    public class ElasticSearchTarget : TargetWithLayout
    {
        #region DI

        public ElasticSearchTarget()
        {

        }

        #endregion

        #region 私有成员

        ElasticsearchClient _Elasticsearch;

        ElasticsearchClient Elasticsearch
        {
            get
            {
                if (_Elasticsearch == null)
                    _Elasticsearch = AutofacHelper.GetService<IElasticsearchProvider>()
                                            .GetElasticsearch<System_Log>();
                return _Elasticsearch;
            }
        }

        System_Log GetBase_SysLogInfo(LogEventInfo logEventInfo)
        {
            logEventInfo.Properties.TryGetValue(NLoggerConfig.Data, out object data);
            logEventInfo.Properties.TryGetValue(NLoggerConfig.LogType, out object logType);
            logEventInfo.Properties.TryGetValue(NLoggerConfig.CreatorId, out object creatorId);
            logEventInfo.Properties.TryGetValue(NLoggerConfig.CreatorName, out object creatorName);

            return new System_Log
            {
                Id = IdHelper.NextIdUpper(),
                Data = (string)data,
                Level = logEventInfo.Level.ToString(),
                LogContent = logEventInfo.Message,
                LogType = (string)logType,
                CreateTime = logEventInfo.TimeStamp,
                CreatorId = (string)creatorId,
                CreatorName = (string)creatorName
            };
        }

        protected override async void Write(LogEventInfo logEvent)
        {
            var log = GetBase_SysLogInfo(logEvent);
            Monitor(log, logEvent.FormattedMessage);
            await Elasticsearch.AddAsync(log);
        }

        IHubContext<LogHub> LogHub
        {
            get
            {
                if (_LogHub == null)
                    _LogHub = AutofacHelper.GetService<IHubContext<LogHub>>();
                return _LogHub;
            }
        }

        IHubContext<LogHub> _LogHub;

        /// <summary>
        /// 监听
        /// </summary>
        /// <param name="log"></param>
        /// <param name="formattedMessage">格式化后的信息</param>
        async void Monitor(System_Log log, string formattedMessage)
        {
            try
            {
                await LogHub.Clients.All.SendCoreAsync(LogHubMethod.Log,
                    new object[] {
                        log.CreateTime,
                        log.Level,
                        log.LogType,
                        formattedMessage });
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch
            {

            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        #endregion
    }
}

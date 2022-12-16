using Business.Handler;
using Entity.System;
using Microservice.Library.Container;
using Microservice.Library.Elasticsearch;
using Microservice.Library.Elasticsearch.Gen;
using Microservice.Library.Extension;
using Model.Utils.Config;
using Model.Utils.Log;
using Model.Utils.SignalR;
using NLog;
using NLog.Targets;
using System.Threading.Tasks;

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

        static System_Log GetBase_SysLogInfo(LogEventInfo logEventInfo)
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
            if (!logEvent.Properties.TryGetValue(NLoggerConfig.LogType, out object logType) && logEvent.Exception != null)
            {
                logEvent.Message += logEvent.Exception == null ? "" : $"\r\n\t{logEvent.Exception.GetExceptionAllMsg()}";
                logEvent.Properties[NLoggerConfig.LogType] = LogType.GetName(LogType.系统异常);
                logEvent.Properties[NLoggerConfig.Data] = "\r\n\tStack Trace: " + logEvent.Exception.ExceptionToString();
            }

            var log = GetBase_SysLogInfo(logEvent);
            Monitor(log, logEvent.FormattedMessage);
            await Elasticsearch.AddAsync(log);
        }

        LogForwardHandler LogForward
        {
            get
            {
                if (_LogForward == null)
                    _LogForward = AutofacHelper.GetService<LogForwardHandler>();
                return _LogForward;
            }
        }

        LogForwardHandler _LogForward;

        /// <summary>
        /// 监听
        /// </summary>
        /// <param name="log"></param>
        /// <param name="formattedMessage">格式化后的信息</param>
        async void Monitor(System_Log log, string formattedMessage)
        {
            try
            {
                LogForward.Add(new LogInfo
                {
                    CreateTime = log.CreateTime,
                    Level = log.Level,
                    LogType = log.LogType,
                    Data = formattedMessage
                });

                await Task.CompletedTask;
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

using Business.Hub;
using Entity.System;
using FreeSql;
using Microservice.Library.Container;
using Microservice.Library.FreeSql.Gen;
using Microsoft.AspNetCore.SignalR;
using Model.Utils.Config;
using Model.Utils.SignalR;
using NLog;
using NLog.Targets;

namespace Business.Utils.Log
{
    public class RDBMSTarget : TargetWithLayout
    {
        #region DI

        public RDBMSTarget()
        {

        }

        IHubContext<LogHub> LogHub => AutofacHelper.GetScopeService<IHubContext<LogHub>>();

        #endregion

        #region 私有成员

        IBaseRepository<System_Log, string> Repository;

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
            await GetRepository().InsertAsync(log);
        }

        private IBaseRepository<System_Log, string> GetRepository()
        {
            if (Repository == null)
                Repository = AutofacHelper.GetService<IFreeSqlProvider>()
                                            .GetFreeSql()
                                            .GetRepository<System_Log, string>();
            return Repository;
        }

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
                await LogHub.Clients.All.SendCoreAsync(LogHubMethod.Log, new[] { formattedMessage });
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

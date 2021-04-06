using Business.Handler;
using Microservice.Library.Container;
using Model.Utils.Config;
using Model.Utils.SignalR;
using NLog;
using NLog.Targets;
using System.Threading.Tasks;

namespace Business.Utils.Log
{
    /// <summary>
    /// 
    /// </summary>
    public class MonitorFileTarget : FileTarget
    {
        protected override string GetFormattedMessage(LogEventInfo logEvent)
        {
            var formattedMessage = base.GetFormattedMessage(logEvent);
            Monitor(logEvent, formattedMessage);
            return formattedMessage;
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
        /// <param name="logEvent"></param>
        /// <param name="formattedMessage">格式化后的信息</param>
        async void Monitor(LogEventInfo logEvent, string formattedMessage)
        {
            try
            {
                logEvent.Properties.TryGetValue(NLoggerConfig.LogType, out object logType);
                LogForward.Add(new LogInfo
                {
                    CreateTime = logEvent.TimeStamp,
                    Level = logEvent.Level.ToString(),
                    LogType = (string)logType,
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
    }
}

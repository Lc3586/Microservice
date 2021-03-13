using Business.Hub;
using Microservice.Library.Container;
using Microsoft.AspNetCore.SignalR;
using Model.Utils.SignalR;
using NLog;
using NLog.Targets;

namespace Business.Utils.Log
{
    /// <summary>
    /// 
    /// </summary>
    public class MonitorFileTarget : FileTarget
    {
        IHubContext<LogHub> LogHub => AutofacHelper.GetScopeService<IHubContext<LogHub>>();

        protected override string GetFormattedMessage(LogEventInfo logEvent)
        {
            var formattedMessage = base.GetFormattedMessage(logEvent);
            Monitor(logEvent, formattedMessage);
            return formattedMessage;
        }

        /// <summary>
        /// 监听
        /// </summary>
        /// <param name="logEvent"></param>
        /// <param name="formattedMessage">格式化后的信息</param>
        async void Monitor(LogEventInfo logEvent, string formattedMessage)
        {
            try
            {
                await LogHub.Clients.All.SendCoreAsync(LogHubMethod.Log, new[] { formattedMessage });
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch
            {

            }
#pragma warning restore CA1031 // Do not catch general exception types
        }
    }
}

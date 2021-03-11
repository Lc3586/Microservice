using Microsoft.AspNetCore.Authorization;
using SignalrHub = Microsoft.AspNetCore.SignalR.Hub;

namespace Business.Hub
{
    /// <summary>
    /// 日志中心
    /// </summary>
    [Authorize]
    public class LogHub : SignalrHub
    {
        public LogHub()
        {

        }
    }
}

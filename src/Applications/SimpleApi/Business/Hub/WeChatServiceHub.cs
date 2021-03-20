using Microservice.Library.Cache.Gen;
using Microservice.Library.Cache.Services;
using Microservice.Library.Container;
using Microservice.Library.WeChat.Extension;
using Microsoft.AspNetCore.SignalR;
using Model.Utils.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SignalrHub = Microsoft.AspNetCore.SignalR.Hub;

namespace Business.Hub
{
    /// <summary>
    /// 微信服务集线器
    /// </summary>
    public class WeChatServiceHub : SignalrHub
    {
        public WeChatServiceHub()
        {

        }

        /// <summary>
        /// 客户端设置
        /// </summary>
        public static readonly ConcurrentDictionary<string, string> Settings
            = new ConcurrentDictionary<string, string>();

        #region 远程方法

        /// <summary>
        /// 设置state参数
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public async Task<bool> SetState(string state)
        {
            if (string.IsNullOrWhiteSpace(state))
                return await Task.FromResult(false);

            Settings.AddOrUpdate(
                state,
                Context.ConnectionId,
                (key, old) => Context.ConnectionId);

            return await Task.FromResult(true);
        }

        #endregion
    }
}

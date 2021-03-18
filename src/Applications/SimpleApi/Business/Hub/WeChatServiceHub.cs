using Microservice.Library.Cache.Gen;
using Microservice.Library.Cache.Services;
using Microservice.Library.Container;
using Microservice.Library.WeChat.Extension;
using Microsoft.AspNetCore.SignalR;
using Model.Utils.SignalR;
using System;
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

        #region DI

        ICache Cache
        {
            get
            {
                if (_Cache == null)
                    _Cache = AutofacHelper.GetScopeService<ICacheProvider>()
                                        .GetCache();
                return _Cache;
            }
        }

        ICache _Cache;

        IWeChatOAuthHandler WeChatOAuthHandler
        {
            get
            {
                if (_WeChatOAuthHandler == null)
                    _WeChatOAuthHandler = AutofacHelper.GetScopeService<IWeChatOAuthHandler>();
                return _WeChatOAuthHandler;
            }
        }

        IWeChatOAuthHandler _WeChatOAuthHandler;

        #endregion

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

            if (!Cache.ContainsKey(state))
                return await Task.FromResult(false);

            await Groups.AddToGroupAsync(Context.ConnectionId, state);

            return await Task.FromResult(true);
        }

        /// <summary>
        /// 系统用户绑定微信的链接被扫描了
        /// </summary>
        public void Confirm()
        {

        }

        #endregion
    }
}

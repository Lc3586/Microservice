using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Utils.SignalR
{
    /// <summary>
    /// 微信服务中心前端方法
    /// </summary>
    public struct WeChatServiceHubMethod
    {
        /// <summary>
        /// 链接已被扫描
        /// </summary>
        public const string Scanned = "Scanned";

        /// <summary>
        /// 已执行确认操作
        /// </summary>
        public const string Confirm = "Confirm";
    }
}

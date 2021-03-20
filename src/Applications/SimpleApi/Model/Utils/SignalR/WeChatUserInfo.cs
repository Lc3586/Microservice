using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Utils.SignalR
{
    /// <summary>
    /// 微信用户信息
    /// </summary>
    public class WeChatUserInfo
    {
        /// <summary>
        /// 用户公众号唯一标识
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public byte Sex { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string HeadimgUrl { get; set; }
    }
}

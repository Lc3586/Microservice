using System.Collections.Generic;

namespace Model.Utils.SignalR
{
    /// <summary>
    /// 客户端日志设置
    /// </summary>
    public class LogSetting
    {
        /// <summary>
        /// 推送
        /// </summary>
        public bool Push { get; set; }

        /// <summary>
        /// 级别
        /// </summary>
        public List<string> Level { get; set; } = new List<string>();

        /// <summary>
        /// 类型
        /// </summary>
        public List<string> Type { get; set; } = new List<string>();

        /// <summary>
        /// 过滤
        /// </summary>
        public List<string> Filter { get; set; } = new List<string>();

        /// <summary>
        /// 关键词
        /// </summary>
        public List<string> Keyword { get; set; } = new List<string>();
    }
}

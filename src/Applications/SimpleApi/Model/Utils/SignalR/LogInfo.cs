using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Utils.SignalR
{
    /// <summary>
    /// 日志信息
    /// </summary>
    public class LogInfo
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 级别
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string LogType { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public string Data { get; set; }
    }
}

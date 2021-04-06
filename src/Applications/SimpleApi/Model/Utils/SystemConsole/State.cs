using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Utils.SystemConsole
{
    /// <summary>
    /// 状态
    /// </summary>
    public static class State
    {
        public const string 未启用 = "none";

        public const string 空闲 = "free";

        public const string 运行中 = "run";

        public const string 已停止 = "stop";
    }
}

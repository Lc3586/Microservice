using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Utils.SignalR
{
    /// <summary>
    /// 分片文件合并任务中心前端方法
    /// </summary>
    public struct CFMTHubMethod
    {
        public const string 新增任务 = "AddTask";

        public const string 更新任务 = "UpdateTask";

        public const string 移除任务 = "RemoveTask";

        public const string 更新分片来源信息 = "UpdateChunksSource";
    }
}

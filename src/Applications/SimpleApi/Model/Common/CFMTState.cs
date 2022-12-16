using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Common
{
    /// <summary>
    /// 分片文件合并任务状态
    /// </summary>
    public static class CFMTState
    {
        public const string 上传中 = "上传中";

        public const string 等待处理 = "等待处理";

        public const string 处理中 = "处理中";

        public const string 已完成 = "已完成";

        public const string 失败 = "失败";
    }
}

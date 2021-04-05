using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Common
{
    /// <summary>
    /// 预备上传分片文件输出状态
    /// </summary>
    public static class PUCFRState
    {
        public const string 允许上传 = "allow";

        public const string 推迟上传 = "delay";

        public const string 跳过 = "pass";

        public const string 全部跳过 = "pass-all";
    }
}

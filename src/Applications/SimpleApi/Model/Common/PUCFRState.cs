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
        public const string 允许上传 = "允许上传";

        public const string 推迟上传 = "推迟上传";

        public const string 跳过 = "跳过";

        public const string 全部跳过 = "全部跳过";
    }
}

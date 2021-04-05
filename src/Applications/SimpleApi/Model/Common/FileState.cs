using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Common
{
    /// <summary>
    /// 文件状态
    /// </summary>
    public static class FileState
    {
        public const string 未上传 = "none";

        public const string 上传中 = "uploading";

        public const string 处理中 = "processing";

        public const string 可用 = "available";

        public const string 已删除 = "deleted";
    }
}

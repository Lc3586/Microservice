using System.Collections.Generic;

namespace Model.Utils.Config
{
    /// <summary>
    /// 接口版本配置
    /// </summary>
    public class ApiVersionSetting
    {
        /// <summary>
        /// 通过Header或QueryString设置版本号时的关键字名称
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// 请求时如果未指定版本则使用的默认版本
        /// </summary>
        public List<int> DefaultVersion { get; set; }

        /// <summary>
        /// 请求时如果未指定版本则指定为当前最高版本
        /// </summary>
        public bool SelectMaximumVersion { get; set; }
    }
}

using T4CAGC.Model;

namespace T4CAGC.Template
{
    /// <summary>
    /// 业务接口类模板选项
    /// </summary>
    public class InterfaceOptions
    {
        /// <summary>
        /// 版本
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 表信息
        /// </summary>
        public TableInfo Table { get; set; }
    }
}

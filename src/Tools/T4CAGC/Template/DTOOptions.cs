using T4CAGC.Model;

namespace T4CAGC.Template
{
    /// <summary>
    /// 业务模型类模板选项
    /// </summary>
    public class DTOOptions
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

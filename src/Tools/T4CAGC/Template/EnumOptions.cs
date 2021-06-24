using T4CAGC.Model;

namespace T4CAGC.Template
{
    /// <summary>
    /// 枚举定义类模板选项
    /// </summary>
    public class EnumOptions
    {
        /// <summary>
        /// 版本
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 表信息注释
        /// </summary>
        public string TableRemark { get; set; }

        /// <summary>
        /// 模块名
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// 简短名称
        /// </summary>
        public string ReducedName { get; set; }

        /// <summary>
        /// 字段信息
        /// </summary>
        public FieldInfo Field { get; set; }
    }
}

using System.Collections.Generic;

namespace T4CAGC.Template
{
    /// <summary>
    /// 枚举定义类模板
    /// </summary>
    public partial class Enum
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options">选项</param>
        public Enum(EnumOptions options)
        {
            Options = options;
        }

        /// <summary>
        /// 选项
        /// </summary>
        readonly EnumOptions Options;

        /// <summary>
        /// 命名空间
        /// </summary>
        readonly List<string> NameSpaces = new List<string>();
    }
}

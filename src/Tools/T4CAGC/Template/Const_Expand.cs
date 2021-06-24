using System.Collections.Generic;

namespace T4CAGC.Template
{
    /// <summary>
    /// 常量定义类模板
    /// </summary>
    public partial class Const
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options">选项</param>
        public Const(ConstOptions options)
        {
            Options = options;
        }

        /// <summary>
        /// 选项
        /// </summary>
        readonly ConstOptions Options;

        /// <summary>
        /// 命名空间
        /// </summary>
        readonly List<string> NameSpaces = new List<string>();
    }
}

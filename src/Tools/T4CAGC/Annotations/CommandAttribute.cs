using System;

namespace T4CAGC.Annotations
{
    /// <summary>
    /// 命令行属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">名称</param>
        public CommandAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        /// <remarks>ASC</remarks>
        public string Sort { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string Description { get; set; }
    }
}

using System.Collections.Generic;

namespace T4CAGC.Config
{
    /// <summary>
    /// 模块配置
    /// </summary>
    public class ModularSetting
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 文件路径
        /// <para>相对路径</para>
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 排序值
        /// <para>排序规则：升序</para>
        /// </summary>
        public int Sort { get; set; } = 0;

        ///// <summary>
        ///// 方法配置信息
        ///// </summary>
        //public List<Method> Methods { get; set; } = new List<Method>();

        /// <summary>
        /// 参数配置信息
        /// </summary>
        public List<Arg> Args { get; set; } = new List<Arg>();

        /// <summary>
        /// 说明
        /// </summary>
        public string Description { get; set; }
    }
}

namespace T4CAGC.Config
{
    /// <summary>
    /// 方法配置
    /// </summary>
    public class MethodSetting
    {
        /// <summary>
        /// 名称
        /// <para>命令行匹配用的名称</para>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 所在类名称
        /// <para>包含命名空间</para>
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 明确的名称
        /// <para>代码中的名称</para>
        /// </summary>
        public string SpecifiedName { get; set; }

        /// <summary>
        /// 静态
        /// </summary>
        public bool Static { get; set; } = false;

        /// <summary>
        /// 实例化时调用构造函数时携带配置作为参数
        /// <para>启用时，目标类必须同时具有无参构造函数</para>
        /// </summary>
        public bool IConfig { get; set; } = false;

        /// <summary>
        /// 异步
        /// </summary>
        public bool Async { get; set; } = false;

        /// <summary>
        /// 参数数组转模型
        /// <para>使用完整名称匹配（忽略大小写）</para>
        /// </summary>
        public Arg2Model Arg2Model { get; set; }

        /// <summary>
        /// 排序值
        /// <para>排序规则：升序</para>
        /// </summary>
        public int Sort { get; set; } = 0;

        /// <summary>
        /// 说明
        /// </summary>
        public string Description { get; set; }
    }
}

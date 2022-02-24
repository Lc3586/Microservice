using System.ComponentModel;

namespace Model.Utils.Sort
{
    /// <summary>
    /// 排序方法
    /// </summary>
    public enum SortMethod
    {
        /// <summary>
        /// 置顶
        /// </summary>
        [Description("置顶")]
        top,
        /// <summary>
        /// 向上
        /// </summary>
        [Description("向上")]
        up,
        /// <summary>
        /// 向下
        /// </summary>
        [Description("向下")]
        down,
        /// <summary>
        /// 置底
        /// </summary>
        [Description("置底")]
        low
    }
}

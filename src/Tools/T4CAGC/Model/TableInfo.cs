using System.Collections.Generic;

namespace T4CAGC.Model
{
    /// <summary>
    /// 表格信息
    /// </summary>
    public class TableInfo
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string Remark { get; set; } = string.Empty;

        /// <summary>
        /// 字段集合
        /// </summary>
        public List<FieldInfo> Fields { get; set; }
    }
}

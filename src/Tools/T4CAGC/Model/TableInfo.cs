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
        /// 模块名
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// 简短名称
        /// </summary>
        public string ReducedName { get; set; }

        /// <summary>
        /// FreeSql实体类
        /// </summary>
        public bool FreeSql { get; set; }

        /// <summary>
        /// Elasticsearch实体类
        /// </summary>
        public bool Elasticsearch { get; set; }

        /// <summary>
        /// 树状结构
        /// </summary>
        public bool Tree { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string Remark { get; set; } = string.Empty;

        /// <summary>
        /// 字段集合
        /// </summary>
        public List<FieldInfo> Fields { get; set; } = new List<FieldInfo>();

        /// <summary>
        /// 关系表
        /// </summary>
        public bool RelationshipTable { get; set; }
    }
}

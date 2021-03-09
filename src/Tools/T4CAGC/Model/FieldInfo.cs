using System;
using System.Collections.Generic;

namespace T4CAGC.Model
{
    /// <summary>
    /// 字段信息
    /// </summary>
    public class FieldInfo
    {
        /// <summary>
        /// 字段名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 主键
        /// </summary>
        public bool Primary { get; set; } = false;

        /// <summary>
        /// 关联数据字段
        /// </summary>
        public bool Virtual { get; set; }

        /// <summary>
        /// 一对一关联标记
        /// </summary>
        public bool FK { get; set; }

        /// <summary>
        /// 多对多关联标记
        /// </summary>
        public bool RK { get; set; }

        /// <summary>
        /// 关联标识
        /// </summary>
        public string KValue { get; set; }

        /// <summary>
        /// 关联数据
        /// </summary>
        public string Bind { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 注释
        /// </summary>
        public string Remark { get; set; } = string.Empty;

        /// <summary>
        /// 索引类型
        /// </summary>
        public IndexType Index { get; set; } = IndexType.None;

        /// <summary>
        /// 是否可为空
        /// </summary>
        public bool Nullable { get; set; } = false;

        /// <summary>
        /// 数据类型
        /// </summary>
        public Type DataType { get; set; } = typeof(string);

        /// <summary>
        /// 长度
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// 小数位
        /// </summary>
        public int Scale { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public string DbType { get; set; }

        /// <summary>
        /// 非空验证
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public List<string> Tag { get; set; }

        /// <summary>
        /// 标签属性
        /// </summary>
        /// <remarks>匹配<see cref="Microservice.Library.OpenApi.Annotations.OpenApiSchemaFormat"/>属性名</remarks>
        public string OASF { get; set; }
    }
}

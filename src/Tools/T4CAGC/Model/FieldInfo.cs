using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// 注释
        /// </summary>
        public string Remark { get; set; } = string.Empty;

        /// <summary>
        /// 说明
        /// </summary>
        public string Description { get; set; } = string.Empty;

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
        /// 精度
        /// </summary>
        public List<int> Precision { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public List<string> Tag { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        /// <remarks>匹配<see cref="Microservice.Library.OpenApi.Annotations.OpenApiSchemaFormat"/>属性名</remarks>
        public string OAS { get; set; }


    }
}

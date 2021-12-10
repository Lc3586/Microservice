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
        /// 一对多关联标记
        /// </summary>
        public bool FRK { get; set; }

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
        public string Description { get; set; }

        /// <summary>
        /// 注释
        /// </summary>
        public string Remark { get; set; } = string.Empty;

        /// <summary>
        /// 索引名称
        /// </summary>
        public string IndexName { get; set; }

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
        public string Type { get; set; } = "string";

        /// <summary>
        /// 数据类型
        /// </summary>
        public Type CsType { get; set; } = typeof(string);

        /// <summary>
        /// 数据类型关键字
        /// </summary>
        public string CsTypeKeyword { get; set; } = "string";

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
        /// 标签集合
        /// </summary>
        public List<string> Tags { get; set; }

        /// <summary>
        /// 接口架构属性
        /// </summary>
        /// <remarks>
        /// <para>以下属性按顺序填写，并以逗号拼接（填写示例：$OAS[number,number_decimal,123456.0789] 或者 $OAS[number,number_decimal]）</para>
        /// <para><see cref="Microservice.Library.OpenApi.Annotations.OpenApiSchemaType"/></para>
        /// <para><see cref="Microservice.Library.OpenApi.Annotations.OpenApiSchemaFormat"/>（可选）</para>
        /// <para>默认值（可选）</para>
        /// </remarks>
        public string[] OAS { get; set; }

        /// <summary>
        /// 接口架构时间格式化
        /// </summary>
        public string OASDTF { get; set; }

        /// <summary>
        /// NEST属性
        /// </summary>
        public string NEST { get; set; }

        /// <summary>
        /// 数据映射集合
        /// </summary>
        /// <remarks>类型/字段名称</remarks>
        public Dictionary<Type, string> Maps { get; set; } = new Dictionary<Type, string>();

        /// <summary>
        /// 常量集合
        /// </summary>
        public Dictionary<string, string> Consts { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// 枚举集合
        /// </summary>
        public Dictionary<string, int?> Enums { get; set; } = new Dictionary<string, int?>();
    }
}

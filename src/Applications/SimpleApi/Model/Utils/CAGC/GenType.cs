using System.ComponentModel;

namespace Model.Utils.CAGC
{
    /// <summary>
    /// 生成类型
    /// </summary>
    public enum GenType
    {
        /// <summary>
        /// 完整项目
        /// </summary>
        [Description("完整项目")]
        CompleteProject,
        /// <summary>
        /// 小型项目
        /// </summary>
        [Description("小型项目")]
        SmallProject,
        /// <summary>
        /// 填充项目
        /// </summary>
        [Description("填充项目")]
        EnrichmentProject,
        /// <summary>
        /// 控制器类
        /// </summary>
        [Description("控制器类")]
        Controller,
        /// <summary>
        /// 业务实现类
        /// </summary>
        [Description("业务实现类")]
        Implementation,
        /// <summary>
        /// 业务接口类
        /// </summary>
        [Description("业务接口类")]
        Interface,
        /// <summary>
        /// 业务实现类&amp;业务接口类
        /// </summary>
        [Description("业务实现类&业务接口类")]
        Business,
        /// <summary>
        /// 业务模型类
        /// </summary>
        [Description("业务模型类")]
        DTO,
        /// <summary>
        /// 常量定义类
        /// </summary>
        [Description("常量定义类")]
        Const,
        /// <summary>
        /// 枚举定义类
        /// </summary>
        [Description("枚举定义类")]
        Enum,
        /// <summary>
        /// 业务模型类&amp;常量定义类&amp;枚举定义类
        /// </summary>
        [Description("业务模型类&常量定义类&枚举定义类")]
        Model,
        /// <summary>
        /// 实体模型类
        /// </summary>
        [Description("实体模型类")]
        Entity
    }
}

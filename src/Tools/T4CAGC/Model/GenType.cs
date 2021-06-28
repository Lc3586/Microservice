namespace T4CAGC.Model
{
    /// <summary>
    /// 生成类型
    /// </summary>
    public enum GenType
    {
        /// <summary>
        /// 完整项目
        /// </summary>
        CompleteProject,
        /// <summary>
        /// 小型项目
        /// </summary>
        SmallProject,
        /// <summary>
        /// 填充项目
        /// </summary>
        EnrichmentProject,
        /// <summary>
        /// 控制器类
        /// </summary>
        Controller,
        /// <summary>
        /// 业务实现类
        /// </summary>
        Implementation,
        /// <summary>
        /// 业务接口类
        /// </summary>
        Interface,
        /// <summary>
        /// 业务实现类&amp;业务接口类
        /// </summary>
        Business,
        /// <summary>
        /// 业务模型类
        /// </summary>
        DTO,
        /// <summary>
        /// 常量定义类
        /// </summary>
        Const,
        /// <summary>
        /// 枚举定义类
        /// </summary>
        Enum,
        /// <summary>
        /// 业务模型类&amp;常量定义类&amp;枚举定义类
        /// </summary>
        Model,
        /// <summary>
        /// 实体模型类
        /// </summary>
        Entity
    }
}

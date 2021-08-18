namespace Model.Utils.Config
{
    /// <summary>
    /// swagger接口文档版本配置
    /// </summary>
    public class SwaggerApiVersion
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 未配置
        /// </summary>
        /// <returns></returns>
        public static SwaggerApiVersion NotConfigured = new SwaggerApiVersion
        {
            Name = "未配置",
            Title = "未配置",
            Version = "1.0",
            Description = "未在 SystemConfig.Swagger.ApiMultiVersion 中找到指定的接口文档版本配置."
        };
    }
}

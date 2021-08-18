using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Utils.Config
{
    /// <summary>
    /// swagger接口文档分组配置
    /// </summary>
    public class SwaggerApiGroup
    {
        /// <summary>
        /// 分组名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        public string[] Versions { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 未配置
        /// </summary>
        /// <returns></returns>
        public static SwaggerApiGroup NotConfigured = new SwaggerApiGroup
        {
            Title = "未配置",
            Versions = new[] { "1.0" },
            Description = "未在 SystemConfig.Swagger.ApiMultiVersion 中找到指定的分组配置."
        };
    }
}

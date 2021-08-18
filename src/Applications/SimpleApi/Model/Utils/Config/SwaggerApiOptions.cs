using System.Collections.Generic;

namespace Model.Utils.Config
{
    /// <summary>
    /// swagger配置
    /// </summary>
    public class SwaggerApiOptions
    {
        /// <summary>
        /// 接口版本
        /// </summary>
        public List<SwaggerApiVersion> ApiVersions { get; set; }

        /// <summary>
        /// 分组
        /// </summary>
        public List<SwaggerApiGroup> Groups { get; set; }
    }
}

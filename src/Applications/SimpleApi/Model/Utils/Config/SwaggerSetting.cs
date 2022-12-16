using System.Collections.Generic;

namespace Model.Utils.Config
{
    /// <summary>
    /// Swagger配置
    /// </summary>
    public class SwaggerSetting
    {
        /// <summary>
        /// 说明文档相对路径
        /// </summary>
        public List<string> XmlComments { get; set; }

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

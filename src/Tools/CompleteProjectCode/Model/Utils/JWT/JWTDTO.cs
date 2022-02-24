using Microservice.Library.OpenApi.Annotations;
using Newtonsoft.Json;
using System;

/* 
 * JWT相关业务模型
 */
namespace Model.Utils.JWT.JWTDTO
{
    /// <summary>
    /// 令牌信息
    /// </summary>
    public class TokenInfo
    {
        /// <summary>
        /// 访问令牌
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [OpenApiSchema(OpenApiSchemaType.@string, OpenApiSchemaFormat.string_datetime)]
        [JsonConverter(typeof(Microservice.Library.OpenApi.JsonExtension.DateTimeConverter), "yyyy-MM-dd HH:mm:ss.ffff")]
        public DateTime Created { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        [OpenApiSchema(OpenApiSchemaType.@string, OpenApiSchemaFormat.string_datetime)]
        [JsonConverter(typeof(Microservice.Library.OpenApi.JsonExtension.DateTimeConverter), "yyyy-MM-dd HH:mm:ss.ffff")]
        public DateTime Expires { get; set; }
    }
}

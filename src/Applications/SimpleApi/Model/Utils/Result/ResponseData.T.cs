using Microservice.Library.OpenApi.Annotations;

namespace Model.Utils.Result
{
    /// <summary>
    /// 接口输出数据
    /// </summary>
    public class ResponseData<T> : ResponseData
    {
        /// <summary>
        /// 数据
        /// </summary>
        [OpenApiSchema(OpenApiSchemaType.model)]
        public T Data { get; set; }
    }
}

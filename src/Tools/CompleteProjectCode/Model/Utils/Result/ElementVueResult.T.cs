using Microservice.Library.OpenApi.Annotations;

namespace Model.Utils.Result
{
    /// <summary>
    /// ElementVue方案
    /// </summary>
    public class ElementVueResult<T>
    {
        /// <summary>
        /// 成功与否
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 异常代码
        /// </summary>
        /// <remarks><see cref="Utils.Result.ErrorCode"/></remarks>
        public int ErrorCode { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        [OpenApiSchema(OpenApiSchemaType.model)]
        public ElementVueResultData<T> Data { get; set; }
    }
}

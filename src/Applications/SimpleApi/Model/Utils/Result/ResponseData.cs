

namespace Model.Utils.Result
{
    /// <summary>
    /// 接口输出数据
    /// </summary>
    public class ResponseData
    {
        /// <summary>
        /// 操作结果
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 错误代码
        /// <see cref="Model.Utils.Result.ErrorCode"/>
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }
    }
}

namespace Model.Utils.Result
{
    /// <summary>
    /// 开发环境接口输出数据
    /// </summary>
    public class DevelopmentResponseData<T> : ResponseData
    {
        /// <summary>
        /// 返回数据
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        public string ExceptionInfo { get; set; }
    }
}

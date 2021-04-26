using Model.Utils.Result;
using System;

namespace Business.Utils
{
    /// <summary>
    /// 消息异常
    /// </summary>
    public class MessageException : ApplicationException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="innerException">内部异常</param>
        public MessageException(string message, Exception innerException = null)
             : base(message, innerException)
        {
            Code = ErrorCode.business;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="code">错误代码</param>
        /// <param name="innerException">内部异常</param>
        public MessageException(string message, ErrorCode code, Exception innerException = null)
             : base(message, innerException)
        {
            Code = code;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="additionalData">附加数据</param>
        /// <param name="innerException">内部异常</param>
        public MessageException(string message, object additionalData, Exception innerException = null)
             : base(message, innerException)
        {
            Code = ErrorCode.business;
            AdditionalData = additionalData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="additionalData">附加数据</param>
        /// <param name="code">错误代码</param>
        /// <param name="innerException">内部异常</param>
        public MessageException(string message, object additionalData, ErrorCode code, Exception innerException = null)
             : base(message, innerException)
        {
            Code = code;
            AdditionalData = additionalData;
        }

        /// <summary>
        /// 错误代码
        /// </summary>
        public ErrorCode Code { get; set; }

        /// <summary>
        /// 附加数据
        /// </summary>
        public object AdditionalData { get; set; }
    }

    /// <summary>
    /// 数据验证异常
    /// </summary>
    public class ValidationException : Exception
    {
        public ValidationException()
        {

        }

        public ValidationException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="innerException">内部异常</param>
        public ValidationException(string message, Exception innerException = null)
             : base(message, innerException)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="additionalData">数据</param>
        /// <param name="innerException">内部异常</param>
        public ValidationException(object additionalData, Exception innerException = null)
             : base(null, innerException)
        {
            AdditionalData = additionalData;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="additionalData">附加数据</param>
        /// <param name="innerException">内部异常</param>
        public ValidationException(string message, object additionalData, Exception innerException = null)
             : base(message, innerException)
        {
            AdditionalData = additionalData;
        }

        /// <summary>
        /// 附加数据
        /// </summary>
        public object AdditionalData { get; set; }
    }
}

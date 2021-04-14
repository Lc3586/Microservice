namespace Model.Utils.Result
{
    /// <summary>
    /// 接口输出数据工厂类
    /// </summary>
    public static class ResponseDataFactory
    {
        /// <summary>
        /// 返回成功
        /// </summary>
        /// <returns></returns>
        public static ResponseData Success()
        {
            ResponseData res = new ResponseData
            {
                Success = true,
                Message = "操作成功！"
            };

            return res;
        }

        /// <summary>
        /// 返回成功
        /// </summary>
        /// <returns></returns>
        public static ResponseData<T> Success<T>()
        {
            ResponseData<T> res = new ResponseData<T>
            {
                Success = true,
                Message = "操作成功！",
                Data = default
            };

            return res;
        }

        /// <summary>
        /// 返回成功
        /// </summary>
        /// <param name="msg">返回的消息</param>
        /// <returns></returns>
        public static ResponseData Success(string msg)
        {
            ResponseData res = new ResponseData
            {
                Success = true,
                Message = msg
            };

            return res;
        }

        /// <summary>
        /// 返回成功
        /// </summary>
        /// <param name="msg">返回的消息</param>
        /// <returns></returns>
        public static ResponseData SuccessWithDefaultData<T>(string msg)
        {
            ResponseData<T> res = new ResponseData<T>
            {
                Success = true,
                Message = msg,
                Data = default
            };

            return res;
        }

        /// <summary>
        /// 返回成功
        /// </summary>
        /// <param name="data">返回的数据</param>
        /// <returns></returns>
        public static ResponseData<T> Success<T>(T data)
        {
            ResponseData<T> res = new ResponseData<T>
            {
                Success = true,
                Message = "操作成功！",
                Data = data
            };

            return res;
        }

        /// <summary>
        /// 返回成功
        /// </summary>
        /// <param name="msg">返回的消息</param>
        /// <param name="data">返回的数据</param>
        /// <returns></returns>
        public static ResponseData<T> Success<T>(string msg, T data)
        {
            ResponseData<T> res = new ResponseData<T>
            {
                Success = true,
                Message = msg,
                Data = data
            };

            return res;
        }

        /// <summary>
        /// 返回错误
        /// </summary>
        /// <returns></returns>
        public static ResponseData Error()
        {
            ResponseData res = new ResponseData
            {
                Success = false,
                Message = "操作失败！"
            };

            return res;
        }

        /// <summary>
        /// 返回错误
        /// </summary>
        /// <returns></returns>
        public static ResponseData<T> Error<T>()
        {
            ResponseData<T> res = new ResponseData<T>
            {
                Success = false,
                Message = "操作失败！",
                Data = default
            };

            return res;
        }

        /// <summary>
        /// 返回错误
        /// </summary>
        /// <param name="msg">返回的消息</param>
        /// <param name="errorCode">错误代码<see cref="ErrorCode"/></param>
        /// <returns></returns>
        public static ResponseData Error(string msg, ErrorCode errorCode = ErrorCode.none)
        {
            ResponseData res = new ResponseData
            {
                Success = false,
                Message = msg,
                ErrorCode = (int)errorCode
            };

            return res;
        }

        /// <summary>
        /// 返回错误
        /// </summary>
        /// <param name="msg">返回的消息</param>
        /// <param name="errorCode">错误代码<see cref="ErrorCode"/></param>
        /// <returns></returns>
        public static ResponseData<T> Error<T>(string msg, ErrorCode errorCode = ErrorCode.none)
        {
            ResponseData<T> res = new ResponseData<T>
            {
                Success = false,
                Message = msg,
                Data = default,
                ErrorCode = (int)errorCode
            };

            return res;
        }

        /// <summary>
        /// 返回错误
        /// </summary>
        /// <param name="data">返回的数据</param>
        /// <param name="errorCode">错误代码<see cref="ErrorCode"/></param>
        /// <returns></returns>
        public static ResponseData<T> Error<T>(T data, ErrorCode errorCode = ErrorCode.none)
        {
            ResponseData<T> res = new ResponseData<T>
            {
                Success = false,
                Message = "操作失败！",
                Data = data,
                ErrorCode = (int)errorCode
            };

            return res;
        }

        /// <summary>
        /// 返回成功
        /// </summary>
        /// <param name="msg">返回的消息</param>
        /// <param name="data">返回的数据</param>
        /// <param name="errorCode">错误代码<see cref="ErrorCode"/></param>
        /// <returns></returns>
        public static ResponseData<T> Error<T>(string msg, T data, ErrorCode errorCode = ErrorCode.none)
        {
            ResponseData<T> res = new ResponseData<T>
            {
                Success = false,
                Message = msg,
                Data = data,
                ErrorCode = (int)errorCode
            };

            return res;
        }

        /// <summary>
        /// 返回错误（开发模式）
        /// </summary>
        /// <param name="msg">返回的消息</param>
        /// <param name="exMsg">异常信息</param>
        /// <param name="data">返回的数据</param>
        /// <param name="errorCode">错误代码<see cref="ErrorCode"/></param>
        /// <returns></returns>
        public static DevelopmentResponseData<T> Error<T>(string msg, string exMsg, T data, ErrorCode errorCode = ErrorCode.none)
        {
            DevelopmentResponseData<T> res = new DevelopmentResponseData<T>
            {
                Success = false,
                Message = msg,
                Data = data,
                ExceptionInfo = exMsg,
                ErrorCode = (int)errorCode
            };

            return res;
        }
    }
}

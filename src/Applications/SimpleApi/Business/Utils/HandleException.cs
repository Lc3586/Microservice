using Business.Utils.Log;
using Microservice.Library.Container;
using Microservice.Library.Extension;
using Microservice.Library.FreeSql.Application;
using Model.Utils.Config;
using Model.Utils.Log;
using Model.Utils.Result;
using System;
using System.Collections.Generic;

namespace Business.Utils
{
    /// <summary>
    /// 异常帮助类
    /// </summary>
    public static class ExceptionHelper
    {
        readonly static SystemConfig Config = AutofacHelper.GetService<SystemConfig>();

        /// <summary>
        /// 处理系统异常
        /// </summary>
        /// <param name="exception">异常</param>
        /// <param name="url">请求地址</param>
        /// <param name="target">目标</param>
        /// <param name="method">方法</param>
        public static void ExceptionWriteLog(Exception exception, string url = null, string target = null, string method = null)
        {
            Logger.Log(NLog.LogLevel.Error, LogType.系统异常, "请求接口时发生异常.", $"url: {url}, \r\n\tTarget: {target}, \r\n\tMethod: {method}", exception);
        }

        /// <summary>
        /// 处理系统异常
        /// </summary>
        /// <param name="exception">异常</param>
        /// <param name="url">请求地址</param>
        /// <param name="target">目标</param>
        /// <param name="method">方法</param>
        /// <returns></returns>
        public static ResponseData HandleException(Exception exception, string url = null, string target = null, string method = null)
        {
            ExceptionWriteLog(exception, url, target, method);
            return HandleException(exception);
        }

        /// <summary>
        /// 处理系统异常
        /// </summary>
        /// <param name="exception">当前异常</param>
        /// <returns></returns>
        public static ResponseData HandleException(Exception exception)
        {
            ErrorCode code = ErrorCode.business;
            var message = string.Empty;
            var data = new List<object>();

            HandleException(exception);

            if (Config.RunMode != RunMode.Publish && Config.RunMode != RunMode.Publish_Swagger)
                return ResponseDataFactory.Error(
                    string.IsNullOrWhiteSpace(message) ? "系统异常" : message,
                    exception.ExceptionToString(),
                    data,
                    code);
            else
                return data.Count > 0 ?
                    ResponseDataFactory.Error(
                        string.IsNullOrWhiteSpace(message) ? "系统繁忙，请稍后重试" : message,
                        code)
                    : ResponseDataFactory.Error(
                        string.IsNullOrWhiteSpace(message) ? "系统繁忙，请稍后重试" : message,
                        data,
                        code);

            void HandleException(Exception ex)
            {
                Type e_type = ex.GetType();
                if (e_type == typeof(MessageException))
                {
                    var _ex = ex as MessageException;
                    message += _ex.Message;
                    data.Add(_ex.Data);
                    code = _ex.Code;
                }
                else if (e_type == typeof(ValidationException))
                {
                    var _ex = ex as ValidationException;
                    message += _ex.Message;
                    data.Add(_ex.Data);
                    code = ErrorCode.validation;
                }
                else if (e_type == typeof(FreeSqlException))
                {
                    var _ex = ex as FreeSqlException;
                    message += _ex.Message;
                }

                if (ex.InnerException != null)
                    HandleException(ex.InnerException);
            }
        }
    }
}

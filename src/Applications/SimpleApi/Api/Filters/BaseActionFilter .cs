﻿using Business.Interface.System;
using Microservice.Library.Container;
using Microservice.Library.Extension;
using Microsoft.AspNetCore.Mvc;
using Model.Utils.Config;
using Model.Utils.Result;
using System;

namespace Api
{
    /// <summary>
    /// 拦截类基类
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class BaseActionFilter : Attribute
    {
        SystemConfig _Config;

        /// <summary>
        /// 系统配置
        /// </summary>
        protected SystemConfig Config
        {
            get
            {
                if (_Config == null)
                    _Config = AutofacHelper.GetService<SystemConfig>();
                return _Config;
            }
        }

        /// <summary>
        /// 当前操作者
        /// </summary>
        protected IOperator Operator => AutofacHelper.GetScopeService<IOperator>();

        /// <summary>
        /// 返回JSON
        /// </summary>
        /// <param name="json">json字符串</param>
        /// <returns></returns>
        public ContentResult JsonContent(string json)
        {
            return new ContentResult { Content = json, StatusCode = 200, ContentType = "application/json; charset=utf-8" };
        }

        /// <summary>
        /// 返回成功
        /// </summary>
        /// <returns></returns>
        public ContentResult Success()
        {
            ResponseData res = new ResponseData
            {
                Success = true,
                Message = "请求成功！"
            };

            return JsonContent(res.ToJson());
        }

        /// <summary>
        /// 返回成功
        /// </summary>
        /// <param name="msg">消息</param>
        /// <returns></returns>
        public ContentResult Success(string msg)
        {
            ResponseData res = new ResponseData
            {
                Success = true,
                Message = msg
            };

            return JsonContent(res.ToJson());
        }

        /// <summary>
        /// 返回成功
        /// </summary>
        /// <param name="data">返回的数据</param>
        /// <returns></returns>
        public ContentResult Success<T>(T data)
        {
            ResponseData<T> res = new ResponseData<T>
            {
                Success = true,
                Message = "请求成功！",
                Data = data
            };

            return JsonContent(res.ToJson());
        }

        /// <summary>
        /// 返回错误
        /// </summary>
        /// <returns></returns>
        public ContentResult Error()
        {
            ResponseData res = new ResponseData
            {
                Success = false,
                Message = "请求失败！"
            };

            return JsonContent(res.ToJson());
        }

        /// <summary>
        /// 返回错误
        /// </summary>
        /// <param name="msg">错误提示</param>
        /// <returns></returns>
        public ContentResult Error(string msg)
        {
            ResponseData res = new ResponseData
            {
                Success = false,
                Message = msg,
            };

            return JsonContent(res.ToJson());
        }

        /// <summary>
        /// 返回错误
        /// </summary>
        /// <param name="msg">错误提示</param>
        /// <param name="errorCode">错误代码</param>
        /// <returns></returns>
        public ContentResult Error(string msg, ErrorCode errorCode)
        {
            ResponseData res = new ResponseData
            {
                Success = false,
                Message = msg,
                ErrorCode = (int)errorCode
            };

            return JsonContent(res.ToJson());
        }
    }
}
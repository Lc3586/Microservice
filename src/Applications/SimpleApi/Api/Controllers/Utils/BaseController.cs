using Microservice.Library.Extension;
using Microservice.Library.OpenApi.Extention;
using Microsoft.AspNetCore.Mvc;
using Model.Utils.Pagination;
using Model.Utils.Result;
using System.Collections.Generic;
using System.Text;

namespace Api.Controllers.Utils
{
    /// <summary>
    /// 基控制器
    /// </summary>
    [CheckModel]//检查模型
    [JsonParamter(true)]//Json参数转模型
    public class BaseController : ControllerBase
    {
        /// <summary>
        /// 返回JSON
        /// </summary>
        /// <typeparam name="TOpenApiSchema">接口架构类型</typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected ContentResult OpenApiJsonContent<TOpenApiSchema>(TOpenApiSchema obj)
        {
            return base.Content(obj.ToOpenApiJson(), "application/json", Encoding.UTF8);
        }

        /// <summary>
        /// 返回JSON
        /// </summary>
        /// <typeparam name="TOpenApiSchema">接口架构类型</typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        protected ContentResult OpenApiJsonContent<TOpenApiSchema>(object result)
        {
            return base.Content(result.ToOpenApiJson<TOpenApiSchema>(), "application/json", Encoding.UTF8);
        }

        /// <summary>
        /// 返回JSON
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected ContentResult JsonContent(object result)
        {
            return base.Content(result.ToJson(), "application/json", Encoding.UTF8);
        }

        /// <summary>
        /// 返回JSON
        /// </summary>
        /// <typeparam name="TOpenApiSchema">接口架构类型</typeparam>
        /// <param name="obj"></param>
        /// <param name="pagination"></param>
        /// <param name="success"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        protected ContentResult OpenApiJsonContent<TOpenApiSchema>(List<TOpenApiSchema> obj, PaginationDTO pagination, bool success = true, string error = null)
        {
            var type = pagination.GetSchemaResultType<TOpenApiSchema>();
            return base.Content(pagination.BuildResult(obj, success, error).ToOpenApiJson(type), "application/json", Encoding.UTF8);
        }

        /// <summary>
        /// 返回JSON
        /// </summary>
        /// <typeparam name="TOpenApiSchema">接口架构类型</typeparam>
        /// <param name="obj"></param>
        /// <param name="pagination"></param>
        /// <param name="success"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        protected ContentResult OpenApiJsonContent<TOpenApiSchema>(List<object> obj, PaginationDTO pagination, bool success = true, string error = null)
        {
            var type = pagination.GetSchemaResultType<TOpenApiSchema>();
            return base.Content(pagination.BuildResult(obj, success, error).ToOpenApiJson(type), "application/json", Encoding.UTF8);
        }

        /// <summary>
        /// 返回JSON
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="pagination"></param>
        /// <param name="success"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        protected ContentResult JsonContent(List<object> obj, PaginationDTO pagination, bool success = true, string error = null)
        {
            return base.Content(pagination.BuildResult(obj, success, error).ToJson(), "application/json", Encoding.UTF8);
        }

        /// <summary>
        /// 返回JSON
        /// </summary>
        /// <param name="jsonStr">json字符串</param>
        /// <returns></returns>
        protected ContentResult JsonContent(string jsonStr)
        {
            return base.Content(jsonStr, "application/json", Encoding.UTF8);
        }

        /// <summary>
        /// 返回html
        /// </summary>
        /// <param name="body">html内容</param>
        /// <returns></returns>
        protected ContentResult HtmlContent(string body)
        {
            return base.Content(body);
        }

        /// <summary>
        /// 返回成功
        /// </summary>
        /// <returns></returns>
        protected ContentResult Success()
        {
            ResponseData res = new ResponseData
            {
                Success = true,
                Message = "请求成功！",
            };

            return JsonContent(res.ToJson());
        }

        /// <summary>
        /// 返回成功
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns></returns>
        protected ContentResult Success(string message)
        {
            ResponseData res = new ResponseData
            {
                Success = true,
                Message = message,
            };

            return JsonContent(res.ToJson());
        }

        /// <summary>
        /// 返回成功
        /// </summary>
        /// <param name="data">返回的数据</param>
        /// <returns></returns>
        protected ContentResult Success<T>(T data)
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
        /// 返回成功
        /// </summary>
        /// <param name="data">返回的数据</param>
        /// <param name="message">返回的消息</param>
        /// <returns></returns>
        protected ContentResult Success<T>(T data, string message)
        {
            ResponseData<T> res = new ResponseData<T>
            {
                Success = true,
                Message = message,
                Data = data
            };

            return JsonContent(res.ToJson());
        }

        /// <summary>
        /// 返回成功
        /// </summary>
        /// <typeparam name="TOpenApiSchema">接口架构类型</typeparam>
        /// <param name="data"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected ContentResult SuccessOpenApiSchema<TOpenApiSchema>(TOpenApiSchema data, string message = "请求成功")
        {
            return base.Content(new ResponseData<object>
            {
                Success = true,
                Message = message,
                Data = data.ToOpenApiJson().ToObject<object>()
            }.ToJson(), "application/json", Encoding.UTF8);
        }

        /// <summary>
        /// 返回错误
        /// </summary>
        /// <returns></returns>
        protected ContentResult Error()
        {
            ResponseData res = new ResponseData
            {
                Success = false,
                Message = "请求失败！",
            };

            return JsonContent(res.ToJson());
        }

        /// <summary>
        /// 返回错误
        /// </summary>
        /// <param name="message">错误提示</param>
        /// <returns></returns>
        protected ContentResult Error(string message)
        {
            ResponseData res = new ResponseData
            {
                Success = false,
                Message = message,
            };

            return JsonContent(res.ToJson());
        }

        /// <summary>
        /// 返回错误
        /// </summary>
        /// <typeparam name="TOpenApiSchema">接口架构类型</typeparam>
        /// <param name="data"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected ContentResult ErrorOpenApiSchema<TOpenApiSchema>(TOpenApiSchema data, string message = "请求失败")
        {
            return base.Content(new ResponseData<object>
            {
                Success = false,
                Message = message,
                Data = data.ToOpenApiJson().ToObject<object>()
            }.ToJson(), "application/json", Encoding.UTF8);
        }

        /// <summary>
        /// 返回表格数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="list">数据列表</param>
        /// <returns></returns>
        protected ContentResult DataTable<T>(List<T> list)
        {
            return DataTable(list, new PaginationDTO());
        }

        /// <summary>
        /// 返回表格数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="list">数据列表</param>
        /// <param name="pagination">分页参数</param>
        /// <returns></returns>
        protected ContentResult DataTable<T>(List<T> list, PaginationDTO pagination)
        {
            return JsonContent(pagination.BuildResult(list).ToJson());
        }
    }
}
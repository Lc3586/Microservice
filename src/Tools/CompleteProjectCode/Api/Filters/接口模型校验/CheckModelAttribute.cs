﻿using Business.Utils;
using Microservice.Library.Extension;
using Microservice.Library.Http;
using Microservice.Library.OpenApi.Annotations;
using Microservice.Library.OpenApi.Extention;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Model.Utils.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Api
{
    /// <summary>
    /// 接口模型校验
    /// LCTR 2019-12-12
    /// </summary>
    public class CheckModelAttribute : Attribute, IAsyncActionFilter
    {
        public CheckModelAttribute(params string[] ignore)
            : base()
        {
            Ignore = ignore;
        }

        /// <summary>
        /// 忽略字段
        /// </summary>
        public string[] Ignore { get; set; }

        /// <summary>
        /// Action执行之前执行
        /// </summary>
        /// <param name="context">过滤器上下文</param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ContainsFilter<NoCheckModelAttribute>())
                return;

            var hasTag = false;
            List<string> propNames = new List<string>();
            foreach (var parameter in context.ActionDescriptor.Parameters)
            {
                var strictMode = parameter.ParameterType.GetCustomAttribute<OpenApiSchemaStrictModeAttribute>() != null;
                var mainTag = parameter.ParameterType.GetMainTag();
                if (mainTag.IsNullOrEmpty())
                    continue;
                else
                    hasTag = true;
                foreach (var property in parameter.ParameterType.GetProperties())
                {
                    if ((!strictMode && property.DeclaringType?.Name == parameter.ParameterType.Name) || property.HasTag(mainTag))
                        propNames.Add(property.Name);
                }
            }

            var ModelErrors = context.ModelState.Count > 0 ?
                context.ModelState
                    .Where(o =>
                    {
                        if (o.Key.IsNullOrWhiteSpace())
                            return false;

                        var key = o.Key[(o.Key.LastIndexOf('.') + 1)..];
                        return (!hasTag || propNames.Contains(key)) &&
                         (!Ignore.Any_Ex() || !Ignore.Contains(key)) &&
                         o.Value.Errors.Count > 0;
                    })
                    .Select(o =>
                        {
                            var error = new ModelErrorsInfo()
                            {
                                FullKey = o.Key,
                                Key = o.Key[(o.Key.LastIndexOf('.') + 1)..],
                                Value = o.Value.RawValue,
                                Errors = o.Value.Errors.Select(p => p.ErrorMessage).ToList()
                            };
                            return error;
                        })
                    .ToList() : null;

            if (ModelErrors.Any_Ex())
            {
                context.Result = new ContentResult { Content = ResponseDataFactory.Error("数据验证失败", ModelErrors, ErrorCode.validation).ToJson(), ContentType = "application/json;charset=utf-8" };
                return;
            }

            await next.Invoke().ConfigureAwait(false);
        }
    }
}

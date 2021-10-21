using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Api.Middleware
{
    /// <summary>
    /// Cookie同源政策自动更改中间件
    /// </summary>
    public class ChangeCookieSameSiteMiddleware
    {
        public static readonly List<PathString> Paths
            = new List<PathString>();

        private readonly RequestDelegate _next;

        public ChangeCookieSameSiteMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next.Invoke(context).ConfigureAwait(true);

            if (context?.Request.Method.Equals(HttpMethod.Get.Method, StringComparison.OrdinalIgnoreCase) == true
                && Paths.Any(o => context.Request.Path.Equals(o, StringComparison.OrdinalIgnoreCase) == true))
            {
                if (context.Response.Headers.TryGetValue("Set-Cookie", out StringValues cookie))
                {
                    var values = new string[2];
                    for (int i = 0; i < cookie.Count; i++)
                    {
                        if (context.Request.IsHttps)
                        {
                            values[i] = cookie[i].Replace("samesite=strict;", "samesite=none;");
                        }
                        else
                        {
                            values[i] = cookie[i].Replace("samesite=none;", "samesite=strict;");
                        }
                    }
                    context.Response.Headers["Set-Cookie"] = new StringValues(values);
                }
            }
        }
    }
}

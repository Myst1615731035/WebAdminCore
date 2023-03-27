using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SqlSugar.Extensions;
using WebUtils;

namespace WebExtention.Middlewares
{
    /// <summary>
    /// 中间件
    /// 记录请求和响应数据
    /// </summary>
    public class RequRespLogMiddleware
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly RequestDelegate _next;
        private readonly ILogger<RequRespLogMiddleware> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        public RequRespLogMiddleware(RequestDelegate next, ILogger<RequRespLogMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (AppConfig.Get("Middleware", "RequestResponseLog", "Enable").ObjToBool())
            {
                // 过滤，只有接口
                if (context.Request.Path.Value.Contains("api"))
                {
                    context.Request.EnableBuffering();
                    Stream originalBody = context.Response.Body;
                    try
                    {
                        // 存储请求数据
                        if (AppConfig.Get("Middleware", "RequestResponseLog", "RequestLog").ObjToBool()) 
                            await RequestDataLog(context);

                        if (AppConfig.Get("Middleware", "RequestResponseLog", "ResponseLog").ObjToBool())
                        {
                            using (var ms = new MemoryStream())
                            {
                                context.Response.Body = ms;

                                await _next(context);

                                // 存储响应数据
                                ResponseDataLog(context.Response, ms);

                                ms.Position = 0;
                                await ms.CopyToAsync(originalBody);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // 记录异常                        
                        _logger.LogError(ex.Message + "" + ex.InnerException);
                    }
                    finally
                    {
                        context.Response.Body = originalBody;
                    }
                }
                else
                {
                    await _next(context);
                }
            }
            else
            {
                await _next(context);
            }
        }

        private async Task RequestDataLog(HttpContext context)
        {
            var request = context.Request;
            var sr = new StreamReader(request.Body);

            var content = $" QueryData:{request.Path + request.QueryString}\r\n BodyData:{await sr.ReadToEndAsync()}";

            if (!string.IsNullOrEmpty(content))
            {
                LogHelper.WriteLog("RequestResponseLog", content);
                request.Body.Position = 0;
            }
        }

        private void ResponseDataLog(HttpResponse response, MemoryStream ms)
        {
            ms.Position = 0;
            var responseBody = new StreamReader(ms).ReadToEnd();

            // 去除 Html
            var reg = "<[^>]+>";
            var isHtml = Regex.IsMatch(responseBody, reg);

            if (!string.IsNullOrEmpty(responseBody))
            {
                LogHelper.WriteLog("RequestResponseLog", responseBody);
            }
        }
    }
}


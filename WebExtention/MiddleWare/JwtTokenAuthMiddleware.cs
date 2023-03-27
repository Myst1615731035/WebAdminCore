using Microsoft.AspNetCore.Http;
using SqlSugar.Extensions;
using System.IdentityModel.Tokens.Jwt;
using WebUtils;

namespace WebExtention.Middlewares
{
    /// <summary>
    /// 中间件
    /// 原做为自定义授权中间件
    /// 先做检查 header token的使用
    /// </summary>
    public class JwtTokenAuthMiddleware
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly RequestDelegate _next;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        public JwtTokenAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }


        private void PreProceed(HttpContext next)
        {
            //Console.WriteLine($"{DateTime.Now} middleware invoke preproceed");
            //...
        }
        private void PostProceed(HttpContext next)
        {
            //Console.WriteLine($"{DateTime.Now} middleware invoke postproceed");
            //....
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public Task Invoke(HttpContext httpContext)
        {
            PreProceed(httpContext);

            //检测是否包含'Authorization'请求头
            if (!httpContext.Request.Headers.ContainsKey("Authorization"))
            {
                PostProceed(httpContext);
                return _next(httpContext);
            }
            var token = httpContext.Request.Headers.Authorization.ObjToString().Replace("Bearer ", "");
            try
            {
                var jwtHandler = new JwtSecurityTokenHandler();
                if (token.IsNotEmpty() && jwtHandler.CanReadToken(token) && JwtHelper.VerifyToken(token))
                {
                    TokenModelJwt tm = JwtHelper.SerializeJwt(token);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{DateTime.Now} middleware wrong:{e.Message}");
            }
            PostProceed(httpContext);
            return _next(httpContext);
        }

    }

}


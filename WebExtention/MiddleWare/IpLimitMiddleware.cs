using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Builder;
using SqlSugar.Extensions;
using WebUtils;

namespace WebExtention.Middlewares
{
    /// <summary>
    /// ip 限流
    /// </summary>
    public static class IpLimitMiddleware
    {
        public static void UseIpLimitMiddle(this IApplicationBuilder app)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));
            if (AppConfig.Get("Middleware", "IpRateLimit").ObjToBool()) app.UseIpRateLimiting();
        }
    }
}

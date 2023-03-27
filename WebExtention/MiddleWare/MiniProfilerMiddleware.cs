using Microsoft.AspNetCore.Builder;
using SqlSugar.Extensions;
using WebUtils;

namespace WebExtention.Middlewares
{
    /// <summary>
    /// MiniProfiler性能分析
    /// </summary>
    public static class MiniProfilerMiddleware
    {
        public static void UseMiniProfilerMiddleware(this IApplicationBuilder app)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            try
            {
                if (AppConfig.Get("Startup", "MiniProfiler", "Enabled").ObjToBool())
                { 
                    // 性能分析
                    app.UseMiniProfiler();

                }
            }
            catch (Exception e)
            {
                LogHelper.LogException(e);
                throw;
            }
        }
    }
}

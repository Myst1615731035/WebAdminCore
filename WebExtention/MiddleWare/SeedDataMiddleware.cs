using Microsoft.AspNetCore.Builder;
using Utils;

namespace WebSection.Middlewares
{
    /// <summary>
    /// 生成种子数据中间件服务
    /// </summary>
    public static class SeedDataMiddleware
    {
        public static void UseSeedDataMiddle(this IApplicationBuilder app, string webRootPath)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            try
            {
                if (AppConfig.Get("AppSettings", "SeedDBEnabled").ObjToBool() || AppConfig.Get("AppSettings", "SeedDBDataEnabled").ObjToBool())
                {
                    DBSeed.SeedAsync(myContext, webRootPath).Wait();
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

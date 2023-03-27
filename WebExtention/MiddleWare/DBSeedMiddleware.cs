using Microsoft.AspNetCore.Builder;
using SqlSugar;
using SqlSugar.Extensions;
using WebUtils;
using WebUtils.Virtual;

namespace WebExtention.MiddleWare
{
    public static class DBSeedMiddleware
    {
        public static void UseDBSeedMiddleware(this IApplicationBuilder app, IDataSeedBase ds)
        {
            if (app == null) throw new ArgumentNullException();
            try
            {
                ds.DbAsync();
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex);
                throw;
            }
        }
    }
}

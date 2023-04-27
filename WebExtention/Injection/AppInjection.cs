using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebUtils;
using WebUtils.Hubs;
using WebUtils.Virtual;
using WebExtention.MiddleWare;
using WebExtention.Middlewares;
using SqlSugar.Extensions;
using Microsoft.AspNetCore.Rewrite;

namespace WebExtention.Injection
{
    /// <summary>
    /// 程序中间件使用
    /// </summary>
    public static class AppInjection
    {
        public static void UseAppInjection(this WebApplication? app, WebApplicationBuilder builder)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            try
            {
                if (app.Environment.IsDevelopment()) app.UseDeveloperExceptionPage();
                else
                {
                    app.UseExceptionHandler("/Error");
                    //app.UseHsts();
                }
                //限流
                app.UseIpLimitMiddle();
                // 非数据请求的请求路径，跳转到根路径做页面映射
                //app.UsePathBase("/adminpage/");
                app.UseRewriter(new RewriteOptions().Add(new URLRewrite()));
                
                #region MiddleWare
                if (AppConfig.Get("IpRateLimit").ObjToBool()) app.UseIpRateLimiting();
                app.UseMiddleware<RequRespLogMiddleware>();
                app.UseMiddleware<RecordAccessLogsMiddleware>();
                app.UseMiddleware<SignalRSendMiddleware>();
                app.UseMiddleware<IpLogMiddleware>();
                app.UseAllServicesMiddle(builder.Services);
                //app.UseSwaggerAuthorized();
                app.UseSwaggerMiddle();
                #endregion

                #region request pipe
                app.UseSession();
                app.UseCors(AppConfig.Get("HttpRequest", "Cors", "PolicyName"));
                DefaultFilesOptions defaultFilesOptions = new DefaultFilesOptions();
                defaultFilesOptions.DefaultFileNames.Clear();
                defaultFilesOptions.DefaultFileNames.Add("index.html");
                app.UseDefaultFiles(defaultFilesOptions);
                app.UseStaticFiles();
                app.UseCookiePolicy();
                app.UseStatusCodePages();

                app.UseRouting();
                app.UseAuthentication();
                app.UseAuthorization();
                app.UseMiniProfilerMiddleware();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Home}/{action=Index}/{id?}");

                    endpoints.MapHub<ChatHub>("/api2/chatHub");
                });
                #endregion

                #region Scoped DI
                AppConfig.Instance._ServiceProvider = app.Services;
                var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
                var dataSeed = scope.ServiceProvider.GetRequiredService<IDataSeedBase>();
                app.UseDBSeedMiddleware(dataSeed);
                #endregion

                app.Run();
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex);
                Console.WriteLine(ex.ToString());
            }
        }
    }
}

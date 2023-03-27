using Microsoft.AspNetCore.Builder;
using Utils;

namespace WebSection.Middlewares
{
    /// <summary>
    /// Quartz 启动服务
    /// </summary>
    public static class QuartzJobMiddleware
    {
        public static void UseQuartzJobMiddleware(this IApplicationBuilder app, ITasksQzServices tasksQzServices, ISchedulerCenter schedulerCenter)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            try
            {
                if (AppConfig.Get("Middleware", "QuartzNetJob", "Enabled").ObjToBool())
                {

                    var allQzServices = tasksQzServices.Query().Result;
                    foreach (var item in allQzServices)
                    {
                        if (item.IsStart)
                        {
                            var result = schedulerCenter.AddScheduleJobAsync(item).Result;
                            if (result.success)
                            {
                                Console.WriteLine($"QuartzNetJob{item.Name}启动成功！");
                            }
                            else
                            {
                                Console.WriteLine($"QuartzNetJob{item.Name}启动失败！错误信息：{result.msg}");
                            }
                        }
                    }

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

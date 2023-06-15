using Serilog;

namespace WebUtils
{
    public class LogHelper
    {
        private static readonly string path = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
        private static readonly string template = @"时间:{Timestamp:yyyy-MM-dd HH:mm:ss.fff}{NewLine}等级:{Level}{NewLine}来源:{SourceContext}{NewLine}消息:{Message}{NewLine}异常:{Exception}{NewLine}";
        
        /// <summary>
        /// 日志
        /// </summary>
        /// <param name="message"></param>
        public static void LogInfo(string message)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug().Enrich.FromLogContext().WriteTo.Async(config => config.File(
                        Path.Combine(path, "info", ".log"),
                        rollingInterval: RollingInterval.Day,
                        fileSizeLimitBytes: 1024 * 1024 * 10,
                        retainedFileCountLimit: 10,
                        rollOnFileSizeLimit: true,
                        outputTemplate: template,
                        shared: true)
                )
                .CreateLogger();
            Log.Information(message);
            Log.CloseAndFlush();
        }

        /// <summary>
        /// 异常
        /// </summary>
        /// <param name="ex"></param>
        public static void LogException(Exception ex)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug().Enrich.FromLogContext().WriteTo.Async(config => config.File(
                        Path.Combine(path, "err", ".log"),
                        rollingInterval: RollingInterval.Day,
                        fileSizeLimitBytes: 1024 * 1024 * 10,
                        retainedFileCountLimit: 10,
                        rollOnFileSizeLimit: true,
                        outputTemplate: template,
                        shared: true)
                )
                .CreateLogger();
            Log.Error(ex, ex.Message);
            Log.CloseAndFlush();
        }

        /// <summary>
        /// 日志
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="message"></param>
        public static void WriteLog(string dir, string message)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug().Enrich.FromLogContext().WriteTo.Async(config => config.File(
                        Path.Combine(path, dir, ".log"),
                        rollingInterval: RollingInterval.Day,
                        fileSizeLimitBytes: 1024 * 1024 * 10,
                        retainedFileCountLimit: 10,
                        rollOnFileSizeLimit: true,
                        outputTemplate: template,
                        shared: true)
                )
                .CreateLogger();
            Log.Information(message);
            Log.CloseAndFlush();
        }
    }
}

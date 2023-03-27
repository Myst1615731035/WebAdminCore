using Serilog;

namespace WebUtils
{
    public class LogHelper
    {
        private static readonly string path = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
        private static readonly string template = @"时间:{Timestamp:yyyy-MM-dd HH:mm:ss.fff}{NewLine}等级:{Level}{NewLine}来源:{SourceContext}{NewLine}消息:{Message}{NewLine}异常:{Exception}{NewLine}";
        public static void LogInfo(string message)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug().Enrich.FromLogContext().WriteTo.Async(config => config.File(
                        Path.Combine(path, "Info", $"info-"),
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

        public static void LogException(Exception ex)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug().Enrich.FromLogContext().WriteTo.Async(config => config.File(
                        Path.Combine(path, "Exception", $"err-"),
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

        public static void WriteLog(string dir, string message)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug().Enrich.FromLogContext().WriteTo.Async(config => config.File(
                        Path.Combine(path, dir, $"{dir}-{DateTime.Now.ToString("yyyy-MM-dd")}"),
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

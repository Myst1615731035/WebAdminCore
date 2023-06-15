using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Hosting;

namespace WebUtils
{
    /// <summary>
    /// 通过单例声明变量，并获取实时更新的配置文件内容
    /// </summary>
    public class AppConfig
    {
        #region 单例
        private AppConfig() { }
        public static readonly object lockObj = new object();
        
        private static AppConfig _instance;
        public static AppConfig Instance
        {
            get
            {
                if (_instance == null)
                 {
                    lock (lockObj)
                    {
                        if (_instance == null) _instance = new AppConfig();
                    }
                }
                return _instance;
            }
        }
        public IConfiguration? _Configuration { get; set; }
        #endregion

        #region 构造类
        public static IHost? Host { get; set; }
        public static IWebHostEnvironment? Env { get; set; }
        public static IServiceProvider? ServiceProvider { get; set; }
        public static string? ContentRootPath { get; set; }
        public static string? WebRootPath { get; set; }
        public static IConfiguration? Configuration => Instance._Configuration;

        /// <summary>
        /// 通过程序配置项进行构造
        /// </summary>
        /// <param name="configuration"></param>
        public AppConfig(WebApplicationBuilder builder)
        {
            Instance._Configuration = builder.Configuration;
            Env = builder.Environment;
            ContentRootPath = builder.Environment.ContentRootPath;
            WebRootPath = builder.Environment.WebRootPath;
        }

        /// <summary>
        /// 引入文件读取配置
        /// </summary>
        /// <param name="basePath"></param>
        public AppConfig(string basePath)
        {
            //这样的话，可以直接读目录里的json文件，而不是 bin 文件夹下的，所以不用修改复制属性
            _Configuration = new ConfigurationBuilder()
                .Add(new JsonConfigurationSource { Path = basePath, Optional = false, ReloadOnChange = true })
                .Build();
        }
        #endregion

        #region 获取配置
        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="sections"></param>
        /// <returns></returns>
        public static string Get(params string[] sections)
        {
            try
            {
                if (sections.Any()) return Configuration[string.Join(":", sections)];
            }
            catch (Exception) { }
            return "";
        }
        /// <summary>
        /// 获取配置转为实体类对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sections"></param>
        /// <returns></returns>
        public static T Get<T>(params string[] sections) where T : class
        {
            T model = Activator.CreateInstance<T>();
            // 引用 Microsoft.Extensions.Configuration.Binder 包
            Configuration.Bind(string.Join(":", sections), model);
            return model;
        }

        /// <summary>
        /// 获取配置转为List泛型对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sections"></param>
        /// <returns></returns>
        public static List<T> GetList<T>(params string[] sections)
        {
            List<T> list = new List<T>();
            // 引用 Microsoft.Extensions.Configuration.Binder 包
            Configuration.Bind(string.Join(":", sections), list);
            return list;
        }
        #endregion
    }
}

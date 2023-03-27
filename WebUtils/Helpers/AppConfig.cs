using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace WebUtils
{
    public class AppConfig
    {
        #region 构造类
        private static IConfiguration? Configuration { get; set; }
        public static IServiceProvider? serviceProvider { get; set; }
        public static string? ContentRootPath { get; set; }
        public static string? WebRootPath { get; set; }
        /// <summary>
        /// 通过程序配置项进行构造
        /// </summary>
        /// <param name="configuration"></param>
        public AppConfig(IConfiguration configuration, string contentRootPath, string webRootPath)
        {
            Configuration = configuration;
            ContentRootPath = contentRootPath;
            WebRootPath = webRootPath;
        }

        /// <summary>
        /// 引入文件读取配置
        /// </summary>
        /// <param name="path"></param>
        public AppConfig(string path)
        {
            //这样的话，可以直接读目录里的json文件，而不是 bin 文件夹下的，所以不用修改复制属性
            Configuration = new ConfigurationBuilder()
                .Add(new JsonConfigurationSource { Path = path, Optional = false, ReloadOnChange = true })
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
                if (sections.Any())
                {
                    return Configuration[string.Join(":", sections)];
                }
            }
            catch (Exception) { }
            return "";
        }
        public static T Get<T>(params string[] sections)
        {
            T config = default(T);
            Configuration.Bind(string.Join(":", sections), config);
            return config;
        }
        public static List<T> GetList<T>(params string[] sections)
        {
            List<T> list = new List<T>();
            Configuration.Bind(string.Join(":", sections), list);
            return list;
        }
        #endregion
    }
}

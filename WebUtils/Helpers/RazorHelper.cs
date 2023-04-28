using MathNet.Numerics;
using RazorEngineCore;
using System.Collections.Concurrent;

namespace WebUtils
{
    public class RazorHelper
    {
        private static object lockObj = new object();
        private static IRazorEngine razor => new RazorEngine();
        private static ConcurrentDictionary<int, IRazorEngineCompiledTemplate> Cache = new ConcurrentDictionary<int, IRazorEngineCompiledTemplate>();

        /// <summary>
        /// 通过cshtml模板代码获取html内容，同一模板请使用统一的key值
        /// 模板使用一次之后，RazorEngine会对模板进行缓存，可以通过key值直接进行渲染，跳过了模板解析的步骤
        /// 同一模板请使用统一的key值，可以减少程序的内存占用以及模板的渲染时间
        /// </summary>
        /// <param name="template">模板内容</param>
        /// <param name="model">模板内的数据对象</param>
        /// <returns></returns>
        public static async Task<string> Compile(string template, object? model = null, Action<IRazorEngineCompilationOptionsBuilder> builderAction = null)
        {
            try
            {
                var key = template.GetHashCode();
                var html = "";
                if (!Cache.ContainsKey(key))
                {
                    lock (lockObj)
                    {
                        if (!Cache.ContainsKey(key)) Cache.TryAdd(key, razor.Compile(template, builderAction));
                    }
                }
                IRazorEngineCompiledTemplate? compiled = Cache.GetValueOrDefault(key);
                if (compiled.IsNotEmpty()) html = await compiled.RunAsync(model);
                return html;
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex);
                return "";
            }
        }
    }
}

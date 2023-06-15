using RazorEngineCore;
using SqlSugar.Extensions;
using System.Collections.Concurrent;

namespace WebUtils
{
    public class RazorHelper
    {
        private static object lockObj = new object();
        private static IRazorEngine razor => new RazorEngine();
        private static ConcurrentDictionary<int, IRazorEngineCompiledTemplate<RazorTemplate>> Cache = new ConcurrentDictionary<int, IRazorEngineCompiledTemplate<RazorTemplate>>();

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
                        if (!Cache.ContainsKey(key))
                        {
                            var init = razor.Compile<RazorTemplate>(template, builderAction);
                            Cache.TryAdd(key, init);
                        }
                    }
                }
                var compiled = Cache.GetValueOrDefault(key);
                if (compiled.IsNotEmpty()) html = await compiled.RunAsync(t =>
                {
                    t.Model = (model.IsNotEmpty() && model.IsAnonymous()) ? new AnonymousTypeWrapper(model) : model;
                    t.Html = new RazorHtmlHelper(t);
                });
                return html;
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex);
                return "";
            }
        }
    }

    #region 实现Razor模板渲染中的@Html
    public class RazorTemplate : RazorEngineTemplateBase
    {
        public RazorHtmlHelper Html { get; set; }
    }
    public class RazorHtmlHelper
    {
        private readonly RazorTemplate _instance;
        public RazorHtmlHelper(RazorTemplate instance)
        {
            this._instance = instance;
        }

        public string Raw(object? content)
        {
            return content.ObjToString();
        }
    }
    #endregion
}

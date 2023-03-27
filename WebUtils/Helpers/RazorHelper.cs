using RazorEngine;
using RazorEngine.Templating;
using System.Web;

namespace WebUtils
{
    public class RazorHelper
    {
        private static readonly IRazorEngineService razor = Engine.Razor;

        /// <summary>
        /// 通过cshtml模板代码获取html内容，同一模板请使用统一的key值
        /// 模板使用一次之后，RazorEngine会对模板进行缓存，可以通过key值直接进行渲染，跳过了模板解析的步骤
        /// 同一模板请使用统一的key值，可以减少程序的内存占用以及模板的渲染时间
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">模板名称关键字</param>
        /// <param name="template">模板内容</param>
        /// <param name="model">模板内的数据对象</param>
        /// <returns></returns>
        public static async Task<string> Compile<T>(string key, string template, T model)
        {
            return await Task.Run(() =>
            {
                var html = "";
                try
                {
                    if (template.IsNotEmpty())
                    {
                        if (!razor.IsTemplateCached(key, null))
                        {
                            razor.AddTemplate(key, template);
                        }
                        html = razor.RunCompile(key, null, model);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.LogException(ex);
                    html = "";
                }
                return HttpUtility.HtmlDecode(html);
            });
        }
    }
}

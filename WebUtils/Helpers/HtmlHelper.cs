using SqlSugar.Extensions;
using System.Text.RegularExpressions;
using System.Web;

namespace WebUtils
{
    public static class HtmlHelper
    {
        /// <summary>
        /// 对字符串进行HTML格式化
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string ToHtml(this string? html)
        {
            if (html.IsNotEmpty())
            {
                return HttpUtility.HtmlDecode(html.ObjToString());
            }
            return "";
        }

        /// <summary>
        /// 对object进行HTML格式化
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string ToHtml(this object? html)
        {
            if (html.IsNotEmpty())
            {
                return HttpUtility.HtmlDecode(html.ObjToString());
            }
            return "";
        }

        /// <summary>
        /// 获取html内的文本内容，删除所有的html标签
        /// </summary>
        /// <param name="html"></param>
        /// <param name="isCompress"></param>
        /// <returns></returns>
        public static string ToText(this object html, bool isCompress = false)
        {
            return html.ObjToString().ToText();
        }

        public static string ToText(this string html, bool isCompress = false)
        {
            ///删除html内的所有js代码
            var scriptMatch = Regex.Matches(html, "<script>", RegexOptions.IgnoreCase);
            for (int i = 0; i < scriptMatch.Count; i++)
            {
                Match start = Regex.Match(html, "<script>", RegexOptions.IgnoreCase);
                Match end = Regex.Match(html.ToString(), "</script>", RegexOptions.IgnoreCase);
                if (start != null && start.Index > -1)
                {
                    html = html.Remove(start.Index, end.Index + end.Length + -start.Index);
                }
            }
            html = Regex.Replace(html, "<[^>]+>", "");
            html = Regex.Replace(html, "&[^;]+;", "");
            if(isCompress) { html = Regex.Replace(html, "\\s+", " "); }
            return html.Trim();
        }
    }
}

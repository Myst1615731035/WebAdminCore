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
        public static string ToHtml(this object? html)
        {
            return HttpUtility.HtmlDecode(html.ObjToString());
        }

        public static string ToText(this object? data, bool isCompress = false)
        {
            var html = data.ObjToString();
            ///删除html内的所有js代码
            var scriptMatch = Regex.Matches(html, "<script[^>]*>", RegexOptions.IgnoreCase);
            for (int i = 0; i < scriptMatch.Count; i++)
            {
                Match start = Regex.Match(html, "<script[^>]*>", RegexOptions.IgnoreCase);
                Match end = Regex.Match(html.ToString(), "</script>", RegexOptions.IgnoreCase);
                if (start != null && start.Index > -1)
                {
                    html = html.Remove(start.Index, end.Index + end.Length + -start.Index);
                }
            }
            html = Regex.Replace(html, "<[^>]+>", "");
            html = Regex.Replace(html, "&[^;]+;", "");
            if (isCompress) { html = Regex.Replace(html, "\\s+", " "); }
            return html.Trim();
        }
    }
}

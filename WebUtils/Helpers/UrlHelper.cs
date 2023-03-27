using SqlSugar.Extensions;
using System.Text.RegularExpressions;

namespace WebUtils
{
    public class UrlHelper
    {
        /// <summary>
        /// 非数字或大小写字母
        /// </summary>
        private static readonly Regex specialWords = new Regex("[^a-z|A-Z|0-9]");
        /// <summary>
        /// 连续空格
        /// </summary>
        private static readonly Regex mutilSpace = new Regex("(\\s+)");
        /// <summary>
        /// 根据名称删除无法作为url的字符，并生产url和关键值
        /// </summary>
        /// <param name="name"></param>
        /// <returns>item1: url,item2: hashcode</returns>
        public static (string, string) GetUrlTupleWithHashCode(string name)
        {
            if (name.IsEmpty()) return (null, null);
            var code = HashHelper.GetHashCode(name, 4);
            return (mutilSpace.Replace(specialWords.Replace(name, " ").ObjToString().Trim(), "-").ToLower(), code);
        }
        public static string GetUrlWithHashCode(string name, string extention = ".html")
        {
            if (name.IsEmpty()) return null;
            return $"{mutilSpace.Replace(specialWords.Replace(name, " ").ObjToString().Trim(), "-").ToLower()}-{HashHelper.GetHashCode(name, 4)}{extention}";
        }
        public static string GetUrl(string str, string extention = ".html")
        {
            return $"{mutilSpace.Replace(specialWords.Replace(str, " ").ObjToString().Trim(), "-").ToLower()}{extention}";
        }
    }
}

using PluralizeService.Core;
using SqlSugar.Extensions;

namespace WebUtils
{
    /// <summary>
    /// 英文单词辅助类
    /// </summary>
    public static class EnHelper
    {
        /// <summary>
        /// 变单数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToSingular(this string value)
        {
            if (value.IsNotEmpty()) value = PluralizationProvider.Singularize(value);
            return value.ObjToString();
        }

        /// <summary>
        /// 变复数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToPlural(this string value)
        {
            if (value.IsNotEmpty()) value = PluralizationProvider.Pluralize(value);
            return value.ObjToString();
        }
    }
}

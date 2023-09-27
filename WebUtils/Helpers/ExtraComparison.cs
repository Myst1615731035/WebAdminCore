using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebUtils
{
    public class ExtraComparison
    {
        public static readonly IgnoreCaseComparison IgnoreCaseComparison = new IgnoreCaseComparison();
    }

    /// <summary>
    /// 字符串忽略大小写比较器
    /// </summary>
    public sealed class IgnoreCaseComparison : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            return string.Equals(x, y, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(string obj)
        {
            return obj.ToLower().GetHashCode();
        }
    }
}

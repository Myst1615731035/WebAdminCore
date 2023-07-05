using SqlSugar;
using System.Net;
using System.Text.RegularExpressions;

namespace WebUtils
{
    public static class ValidExtention
    {
        public static bool IsInRange(this int thisValue, int begin, int end)
        {
            return thisValue >= begin && thisValue <= end;
        }

        public static bool IsInRange(this DateTime thisValue, DateTime begin, DateTime end)
        {
            return thisValue >= begin && thisValue <= end;
        }

        public static bool IsIn<T>(this T thisValue, params T[] values)
        {
            return values.Contains(thisValue);
        }

        public static bool IsContainsIn(this string thisValue, params string[] inValues)
        {
            return inValues.Any(it => thisValue.Contains(it));
        }

        public static bool IsNullOrEmpty(this object thisValue)
        {
            if (thisValue == null || thisValue == DBNull.Value) return true;
            return thisValue.ToString() == "";
        }

        public static bool IsNullOrEmpty(this Guid? thisValue)
        {
            if (thisValue == null) return true;
            return thisValue == Guid.Empty;
        }

        public static bool IsNullOrEmpty(this Guid thisValue)
        {
            if (thisValue == null) return true;
            return thisValue == Guid.Empty;
        }

        public static bool IsNullOrEmpty(this IEnumerable<object> thisValue)
        {
            if (thisValue == null || thisValue.Count() == 0) return true;
            return false;
        }

        public static bool HasValue(this object thisValue)
        {
            if (thisValue == null || thisValue == DBNull.Value) return false;
            return thisValue.ToString() != "";
        }

        public static bool HasValue(this IEnumerable<object> thisValue)
        {
            if (thisValue == null || thisValue.Count() == 0) return false;
            return true;
        }

        public static bool IsValuable(this IEnumerable<KeyValuePair<string, string>> thisValue)
        {
            if (thisValue == null || thisValue.Count() == 0) return false;
            return true;
        }

        public static bool IsZero(this object thisValue)
        {
            return (thisValue == null || thisValue.ToString() == "0");
        }

        public static bool IsInt(this object thisValue)
        {
            if (thisValue == null) return false;
            return Regex.IsMatch(thisValue.ToString(), @"^\d+$");
        }

        /// <returns></returns>
        public static bool IsNoInt(this object thisValue)
        {
            if (thisValue == null) return true;
            return !Regex.IsMatch(thisValue.ToString(), @"^\d+$");
        }

        public static bool IsMoney(this object thisValue)
        {
            if (thisValue == null) return false;
            double outValue = 0;
            return double.TryParse(thisValue.ToString(), out outValue);
        }
        public static bool IsGuid(this object thisValue)
        {
            if (thisValue == null) return false;
            Guid outValue = Guid.Empty;
            return Guid.TryParse(thisValue.ToString(), out outValue);
        }

        public static bool IsDate(this object thisValue)
        {
            if (thisValue == null) return false;
            DateTime outValue = DateTime.MinValue;
            return DateTime.TryParse(thisValue.ToString(), out outValue);
        }

        public static bool IsEamil(this object thisValue)
        {
            if (thisValue == null) return false;
            return Regex.IsMatch(thisValue.ToString(), @"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$");
        }

        public static bool IsMobile(this object thisValue)
        {
            if (thisValue == null) return false;
            return Regex.IsMatch(thisValue.ToString(), @"^\d{11}$");
        }

        public static bool IsTelephone(this object thisValue)
        {
            if (thisValue == null) return false;
            return System.Text.RegularExpressions.Regex.IsMatch(thisValue.ToString(), @"^(\(\d{3,4}\)|\d{3,4}-|\s)?\d{8}$");

        }

        public static bool IsIDcard(this object thisValue)
        {
            if (thisValue == null) return false;
            return System.Text.RegularExpressions.Regex.IsMatch(thisValue.ToString(), @"^(\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$");
        }

        public static bool IsFax(this object thisValue)
        {
            if (thisValue == null) return false;
            return System.Text.RegularExpressions.Regex.IsMatch(thisValue.ToString(), @"^[+]{0,1}(\d){1,3}[ ]?([-]?((\d)|[ ]){1,12})+$");
        }

        public static bool IsMatch(this object thisValue, string pattern)
        {
            if (thisValue == null) return false;
            Regex reg = new Regex(pattern);
            return reg.IsMatch(thisValue.ToString());
        }
        public static bool IsAnonymousType(this Type type)
        {
            string typeName = type.Name;
            return typeName.Contains("<>") && typeName.Contains("__") && typeName.Contains("AnonymousType");
        }
        public static bool IsCollectionsList(this string thisValue)
        {
            return (thisValue + "").StartsWith("System.Collections.Generic.List") || (thisValue + "").StartsWith("System.Collections.Generic.IEnumerable");
        }
        public static bool IsStringArray(this string thisValue)
        {
            return (thisValue + "").IsMatch(@"System\.[a-z,A-Z,0-9]+?\[\]");
        }
        public static bool IsEnumerable(this string thisValue)
        {
            return (thisValue + "").StartsWith("System.Linq.Enumerable");
        }

        public static Type StringType = typeof(string);

        public static bool IsUrl(this string thisValue)
        {
            var regex = new Regex("^(https?|ftp):\\/\\/[^\\s\\/$.?#].[^\\s]*$");
            return regex.IsMatch(thisValue);
        }

        //IP地址是否为局域网地址的判断函数
        public static bool IsInternalIP(this string? ipAddress)
        {
            IPAddress ip = IPAddress.Parse(ipAddress);
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                byte[] addr = ip.GetAddressBytes();
                if (addr[0] == 10 || (addr[0] == 172 && addr[1] >= 16 && addr[1] <= 31) || (addr[0] == 192 && addr[1] == 168))
                    return true;
                if (addr[0] == 0 && addr[1] == 0 && addr[2] == 0 && addr[3] == 0 && addr[4] == 0 && addr[5] == 0 && addr[6] == 0 && addr[7] == 0 && addr[8] == 0 && addr[9] == 0 && addr[10] == 0xFF && addr[11] == 0xFF)
                    return true;
            }
            return false;
        }
    }
}

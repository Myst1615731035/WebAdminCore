using System.Text;

namespace WebUtils
{
    public class RandomHelper
    {
        public static readonly Random _random = new Random();
        /// <summary>
        /// 生成随机字符串
        /// </summary>
        /// <param name="n">长度</param>
        /// <param name="Number">是否包含数字</param>
        /// <param name="LowerCase">是否包含小写英文字母</param>
        /// <param name="UpperCase">是否包含大写英文字母</param>
        /// <returns></returns>
        public static string GetRandomString(int length, bool Number = true, bool LowerCase = true, bool UpperCase = true)  
        {
            StringBuilder tmp = new StringBuilder(length);
            string characters = (UpperCase ? "ABCDEFGHIJKLMNOPQRSTUVWXYZ" : null) + (Number ? "0123456789" : null) + (LowerCase ? "abcdefghijklmnopqrstuvwxyz" : null);
            if (characters.Length < 1)
            {
                return "";
            }
            for (int i = 0; i < length; i++)
            {
                tmp.Append(characters[_random.Next(0, characters.Length)].ToString());
            }
            return tmp.ToString();
        }
    }
}

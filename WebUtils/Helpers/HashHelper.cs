using System.Security.Cryptography;
using System.Text;

namespace WebUtils
{
    public class HashHelper
    {
        private static readonly string secret = "O4c3fzraovW5RiNLkk8bDL382fZZRlWz";
        public static string GetHashCode(string input, int digits = 6)
        {
            byte[] key = Encoding.UTF8.GetBytes(secret);
            byte[] counter = Encoding.UTF8.GetBytes(input);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(counter);
            }

            HMACSHA1 hmac = new HMACSHA1(key);

            byte[] hash = hmac.ComputeHash(counter);

            int offset = hash[hash.Length - 1] & 0xf;

            int binary =
              ((hash[offset] & 0x7f) << 24)
              | (hash[offset + 1] << 16)
              | (hash[offset + 2] << 8)
              | (hash[offset + 3]);

            int password = binary % (int)Math.Pow(10, digits);
            return password.ToString(new string('0', digits));
        }
    }
}

using System.Security.Cryptography;
using System.Text;

namespace PostGram.Common
{
    public class HashHelper
    {
        public static string GetHash(string input)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sb = new StringBuilder();

                foreach (var item in hash)
                {
                    sb.Append(item.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        public static bool Verify(string input, string hash)
        {
            return hash.Equals(GetHash(input), StringComparison.OrdinalIgnoreCase);
        }
    }
}
using System.Security.Cryptography;
using System.Text;

namespace PostGram.Common
{
    public static class HashHelper
    {
        public static string GetHashSHA256(string input)
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

        public static bool VerifySHA256(string input, string hash)
        {
            return hash.Equals(GetHashSHA256(input), StringComparison.OrdinalIgnoreCase);
        }
    }
}
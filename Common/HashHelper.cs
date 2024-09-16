using System.Security.Cryptography;
using System.Text;

namespace PostGram.Common;

public static class HashHelper
{
    public static string GetHashSha256(string input)
    {
        using var sha = SHA256.Create();
        byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        var stringBuilder = new StringBuilder();

        foreach (byte item in hash)
        {
            stringBuilder.Append(item.ToString("x2"));
        }
        return stringBuilder.ToString();
    }

    public static bool VerifySha256(string input, string hash)
    {
        return hash.Equals(GetHashSha256(input), StringComparison.OrdinalIgnoreCase);
    }
}
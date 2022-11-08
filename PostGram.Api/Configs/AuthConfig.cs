using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace PostGram.Api.Configs
{
    public class AuthConfig
    {
        public static readonly string SectionName = "Auth";
        public string Issuer { get; set; } = String.Empty;
        public string Audience { get; set; } = String.Empty;
        public string Key { get; set; } = String.Empty;
        public int LifeTime { get; set; }

        public SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
        }
    }
}
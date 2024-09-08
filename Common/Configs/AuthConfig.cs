using System.Text;

namespace PostGram.Common.Configs;

public class AuthConfig
{
    public static readonly string SectionName = "Auth";
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public int LifeTime { get; set; }
}
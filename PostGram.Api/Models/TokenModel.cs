namespace PostGram.Api.Models
{
    public class TokenModel
    {
        public TokenModel(string securityToken, string refreshToken)
        {
            SecurityToken = securityToken;
            RefreshToken = refreshToken;
        }

        public string SecurityToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
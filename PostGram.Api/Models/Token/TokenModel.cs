namespace PostGram.Api.Models.Token
{
    public record TokenModel
    {
        public TokenModel(string securityToken, string refreshToken)
        {
            SecurityToken = securityToken;
            RefreshToken = refreshToken;
        }

        public string SecurityToken { get; init; }
        public string RefreshToken { get; init; }
    }
}
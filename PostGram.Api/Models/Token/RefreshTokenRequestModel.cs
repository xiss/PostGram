namespace PostGram.Api.Models.Token
{
    public record RefreshTokenRequestModel
    {
        public RefreshTokenRequestModel(string refreshToken)
        {
            RefreshToken = refreshToken;
        }

        public string RefreshToken { get; init; }
    }
}
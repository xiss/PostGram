namespace PostGram.Api.Models.Token
{
    public record RefreshTokenRequestModel
    {
        public string RefreshToken { get; init; } = string.Empty;
    }
}
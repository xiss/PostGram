namespace PostGram.Api.Models.Token
{
    public record TokenModel
    {
        public string SecurityToken { get; init; } = string.Empty;
        public string RefreshToken { get; init; } = string.Empty;
    }
}
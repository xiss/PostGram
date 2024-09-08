namespace PostGram.Common.Requests;

public record RefreshTokenRequestModel
{
    public string RefreshToken { get; init; } = string.Empty;
}
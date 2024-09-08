namespace PostGram.Common.Dtos.Token;

public record TokenDto
{
    public string SecurityToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
}
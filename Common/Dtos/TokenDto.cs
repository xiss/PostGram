namespace PostGram.Common.Dtos;

public record TokenDto
{
    public string SecurityToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
}
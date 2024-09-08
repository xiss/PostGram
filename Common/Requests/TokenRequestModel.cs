namespace PostGram.Common.Requests;

public record TokenRequestModel
{
    public string Login { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
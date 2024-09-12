namespace PostGram.Common.Requests;

//TODO Что это команда или запрос?
public record RefreshTokenRequestModel
{
    public string RefreshToken { get; init; } = string.Empty;
}
using PostGram.Common.Dtos;

namespace PostGram.BLL.Interfaces.Services;

public interface ITokenService
{
    Task<TokenDto> GetToken(string login, string password);

    Task<TokenDto> GetTokenByRefreshToken(string refreshToken);

    Task<UserSessionDto> GetUserSessionById(Guid id);
}
using PostGram.Common.Models.Token;

namespace PostGram.Common.Interfaces.Services
{
    public interface IAuthService
    {
        Task<TokenModel> GetToken(string login, string password);

        Task<TokenModel> GetTokenByRefreshToken(string refreshToken);

        Task<UserSession> GetUserSessionById(Guid id);

        Task Logout(Guid userId, Guid sessionId);
    }
}
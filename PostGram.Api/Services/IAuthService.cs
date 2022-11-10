using PostGram.Api.Models.Token;
using PostGram.DAL.Entities;

namespace PostGram.Api.Services
{
    public interface IAuthService
    {
        Task<TokenModel> GetToken(string login, string password);
        Task<UserSession> GetUserSessionById(Guid id);
        Task<TokenModel> GetTokenByRefreshToken(string refreshToken);
    }
}
using PostGram.Api.Models;
using PostGram.DAL.Entities;

namespace PostGram.Api.Services
{
    public interface IUserService
    {
        Task<List<UserModel>> GetUsers();
        Task<TokenModel> GetToken(string login, string password);
        Task<UserModel> GetUser(Guid id);
        Task<TokenModel> GetTokenByRefreshToken(string refreshToken);
        Task<Guid> CreateUser(CreateUserModel model);
        Task<UserSession> GetUserSessionById(Guid id);
    }
}
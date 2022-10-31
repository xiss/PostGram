using PostGram.Api.Models;

namespace PostGram.Api.Services
{
    public interface IUserService
    {
        Task CreateUser(CreateUserModel model);
        Task<List<UserModel>> GetUsers();
        Task<TokenModel> GetToken(string login, string password);
        Task<UserModel> GetUser(Guid id);
        Task<TokenModel> GetTokenByRefreshToken(string refreshToken);
    }
}
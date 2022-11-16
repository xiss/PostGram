using PostGram.Api.Models.Attachment;
using PostGram.Api.Models.User;

namespace PostGram.Api.Services
{
    public interface IUserService
    {
        Task<UserModel> GetUser(Guid UserId);

        Task<Guid> CreateUser(CreateUserModel model);

        Task<bool> CheckUserExist(string email);

        Task<Guid> DeleteUser(Guid userId);

        Task AddAvatarToUser(Guid userId, MetadataModel model, string filePath);
        Task<List<UserModel>> GetUsers();
        Task DeleteAvatarForUser(Guid userId);
    }
}
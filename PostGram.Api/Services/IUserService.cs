using PostGram.Api.Models.Attachment;
using PostGram.Api.Models.User;

namespace PostGram.Api.Services
{
    public interface IUserService
    {
        Task AddAvatarToUser(Guid userId, MetadataModel model, string filePath);

        Task<bool> CheckUserExist(string email);

        Task<Guid> CreateUser(CreateUserModel model);

        Task<Guid> DeleteAvatarForUser(Guid userId);

        Task<Guid> DeleteUser(Guid userId);

        Task<UserModel> GetUser(Guid UserId);

        Task<List<UserModel>> GetUsers();
    }
}
using PostGram.Api.Models.Attachment;
using PostGram.Api.Models.User;

namespace PostGram.Api.Services
{
    public interface IUserService
    {
        Task<UserWithAvatarModel> GetUser(Guid id, Func<UserModel, string> linkGenerator);

        Task<Guid> CreateUser(CreateUserModel model);

        Task<bool> CheckUserExist(string email);

        Task<Guid> DeleteUser(Guid userId);

        Task AddAvatarToUser(Guid userId, MetadataModel model, string filePath);
        Task<List<UserWithAvatarModel>> GetUsers(Func<UserModel, string> linkGenerator);
    }
}
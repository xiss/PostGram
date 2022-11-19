using PostGram.Api.Models.Attachment;
using PostGram.Api.Models.Subscription;
using PostGram.Api.Models.User;

namespace PostGram.Api.Services
{
    public interface IUserService
    {
        Task AddAvatarToUser(Guid userId, MetadataModel model, string filePath);

        Task<bool> CheckUserExist(string email);

        Task<Guid> CreateSubscription(CreateSubscriptionModel model, Guid currentUserId);

        Task<Guid> CreateUser(CreateUserModel model);

        Task<Guid> DeleteAvatarForUser(Guid userId);

        Task<Guid> DeleteUser(Guid userId);

        Task<List<SubscriptionModel>> GetMasterSubscriptions(Guid currentUserId);

        Task<List<SubscriptionModel>> GetSlaveSubscriptions(Guid currentUserId);

        Task<UserModel> GetUser(Guid UserId);

        Task<List<UserModel>> GetUsers();

        Task<SubscriptionModel> UpdateSubscription(UpdateSubscriptionModel model, Guid currentUserId);
        Task<UserModel> UpdateUser(UpdateUserModel model, Guid currentUserId);
    }
}
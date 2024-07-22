using PostGram.Common.Models.Attachment;
using PostGram.Common.Models.Subscription;
using PostGram.Common.Models.User;

namespace PostGram.Common.Interfaces.Services
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

        Task<UserModel> GetUser(Guid userId);

        Task<List<UserModel>> GetUsers();

        Task<SubscriptionModel> UpdateSubscription(UpdateSubscriptionModel model, Guid currentUserId);
        Task<UserModel> UpdateUser(UpdateUserModel model, Guid currentUserId);
    }
}
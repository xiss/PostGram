using PostGram.Common.Dtos.Attachment;
using PostGram.Common.Dtos.Subscription;
using PostGram.Common.Dtos.User;
using PostGram.Common.Requests;

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

        Task<List<SubscriptionDto>> GetMasterSubscriptions(Guid currentUserId);

        Task<List<SubscriptionDto>> GetSlaveSubscriptions(Guid currentUserId);

        Task<UserDto> GetUser(Guid userId);

        Task<List<UserDto>> GetUsers();

        Task<SubscriptionDto> UpdateSubscription(UpdateSubscriptionModel model, Guid currentUserId);
        Task<UserDto> UpdateUser(UpdateUserModel model, Guid currentUserId);
    }
}
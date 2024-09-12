using PostGram.Common.Dtos.Attachment;
using PostGram.Common.Dtos.Subscription;
using PostGram.Common.Dtos.User;
using PostGram.Common.Requests.Commands;

namespace PostGram.Common.Interfaces.Services
{
    public interface IUserService
    {
        Task AddAvatarToUser(Guid userId, MetadataModel model, string filePath);

        Task<bool> CheckUserExist(string email);

        Task CreateSubscription(CreateSubscriptionCommand model, Guid currentUserId);

        Task CreateUser(CreateUserCommand model);

        Task DeleteAvatarForUser(Guid userId);

        Task DeleteUser(Guid userId);

        Task<List<SubscriptionDto>> GetMasterSubscriptions(Guid currentUserId);

        Task<List<SubscriptionDto>> GetSlaveSubscriptions(Guid currentUserId);

        Task<UserDto> GetUser(Guid userId);

        Task<List<UserDto>> GetUsers();

        Task UpdateSubscription(UpdateSubscriptionCommand model, Guid currentUserId);

        Task UpdateUser(UpdateUserCommand model, Guid currentUserId);
    }
}
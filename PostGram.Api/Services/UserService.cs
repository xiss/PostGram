using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using PostGram.Api.Models.Attachment;
using PostGram.Api.Models.Subscription;
using PostGram.Api.Models.User;
using PostGram.Common.Exceptions;
using PostGram.DAL;
using PostGram.DAL.Entities;

namespace PostGram.Api.Services
{
    public class UserService : IUserService, IDisposable
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        public UserService(IMapper mapper, DataContext dataContext)
        {
            _mapper = mapper;
            _dataContext = dataContext;
        }

        public async Task AddAvatarToUser(Guid userId, MetadataModel model, string filePath)
        {
            User user = await GetUserById(userId);
            if (user.Avatar != null)
                throw new UnprocessableRequestPostGramException($"User {user.Id} already has avatar {user.AvatarId}. Delete it before add new.");
            Avatar avatar = new Avatar()
            {
                UserId = user.Id,
                AuthorId = user.Id,
                Id = model.TempId,
                MimeType = model.MimeType,
                Name = model.Name,
                Size = model.Size,
            };
            user.Avatar = avatar;
            try
            {
                _dataContext.Avatars.Add(avatar);
                await _dataContext.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException != null)
                {
                    throw new DbPostGramException(e.InnerException.Message, e.InnerException);
                }
                throw new DbPostGramException(e.Message, e);
            }
        }

        public async Task<bool> CheckUserExist(string email)
        {
            return await _dataContext.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<Guid> CreateSubscription(CreateSubscriptionModel model, Guid currentUserId)
        {
            if (!await _dataContext.Users.AnyAsync(u => u.Id == model.MasterId))
                throw new NotFoundPostGramException($"User {model.MasterId} not found");

            if (await _dataContext.Subscriptions.AnyAsync(s => s.MasterId == model.MasterId && s.SlaveId == currentUserId))
                throw new UnprocessableRequestPostGramException(
                    $"Subscription with masterId {model.MasterId} and slaveId {currentUserId} is already exist");

            Subscription subscription = new()
            {
                Id = Guid.NewGuid(),
                Created = DateTimeOffset.UtcNow,
                SlaveId = currentUserId,
                MasterId = model.MasterId,
                Status = false
            };

            try
            {
                await _dataContext.Subscriptions.AddAsync(subscription);
                await _dataContext.SaveChangesAsync();
                return subscription.Id;
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException != null)
                {
                    throw new DbPostGramException(e.InnerException.Message, e.InnerException);
                }
                throw new DbPostGramException(e.Message, e);
            }
        }

        public async Task<Guid> CreateUser(CreateUserModel model)
        {
            if (await CheckUserExist(model.Email))
                throw new DbPostGramException("User with email already exist, email: " + model.Email);
            if (model.BirthDate > DateTimeOffset.UtcNow)
                throw new BadRequestPostGramException("BirthDate cannot be in future");

            User user = _mapper.Map<User>(model);
            try
            {
                await _dataContext.Users.AddAsync(user);
                await _dataContext.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException != null)
                {
                    throw new DbPostGramException(e.InnerException.Message, e.InnerException);
                }
                throw new DbPostGramException(e.Message, e);
            }

            return user.Id;
        }

        public async Task<Guid> DeleteAvatarForUser(Guid userId)
        {
            User user = await GetUserById(userId);
            if (user.Avatar == null)
                throw new NotFoundPostGramException($"User {userId} does't have avatar.");
            Guid avatarId = user.AvatarId!.Value;
            try
            {
                _dataContext.Avatars.Remove(user.Avatar);
                await _dataContext.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException != null)
                {
                    throw new DbPostGramException(e.InnerException.Message, e.InnerException);
                }
                throw new DbPostGramException(e.Message, e);
            }

            return avatarId;
        }

        //TODO 2 Нужно добавить статус у юзера активен, и делать на нем пометку, а при запросах отдавать только по тем юзерам у кого статус активен
        public async Task<Guid> DeleteUser(Guid userId)
        {
            User user = await GetUserById(userId);

            try
            {
                _dataContext.Users.Remove(user);
                await _dataContext.UserSessions.Where(us => us.UserId == userId).ForEachAsync(us => us.IsActive = false);
                await _dataContext.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException != null)
                {
                    throw new DbPostGramException(e.InnerException.Message, e.InnerException);
                }
                throw new DbPostGramException(e.Message, e);
            }

            return userId;
        }

        public void Dispose()
        {
            _dataContext.Dispose();
        }

        public async Task<List<SubscriptionModel>> GetMasterSubscriptions(Guid currentUserId)
        {
            List<SubscriptionModel> subscriptions = await _dataContext.Subscriptions
                .Where(s => s.MasterId == currentUserId)
                .ProjectTo<SubscriptionModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
            if (subscriptions.Count == 0)
                throw new NotFoundPostGramException($"Not found master subscription for user {currentUserId}");
            return subscriptions;
        }

        public async Task<List<SubscriptionModel>> GetSlaveSubscriptions(Guid currentUserId)
        {
            List<SubscriptionModel> subscriptions = await _dataContext.Subscriptions
                .Where(s => s.SlaveId == currentUserId)
                .ProjectTo<SubscriptionModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
            if (subscriptions.Count == 0)
                throw new NotFoundPostGramException($"Not found slave subscription for user {currentUserId}");
            return subscriptions;
        }

        public async Task<UserModel> GetUser(Guid userId)
        {
            User user = await GetUserById(userId);
            return _mapper.Map<UserModel>(user);
        }

        public async Task<List<UserModel>> GetUsers()
        {
            List<UserModel> models = await _dataContext
                .Users
                .Include(x => x.Avatar)
                .AsNoTracking()
                .ProjectTo<UserModel>(_mapper.ConfigurationProvider)
                .ToListAsync();

            if (models.Count == 0)
                throw new NotFoundPostGramException("Users not found in DB");
            return models;
        }

        public async Task<SubscriptionModel> UpdateSubscription(UpdateSubscriptionModel model, Guid currentUserId)
        {
            Subscription? subscription = await _dataContext.Subscriptions.FirstOrDefaultAsync(s => s.Id == model.Id);
            if (subscription == null)
                throw new NotFoundPostGramException($"Not found subscription with id: {model.Id}");

            if (subscription.MasterId != currentUserId && subscription.SlaveId != currentUserId)
                throw new AuthorizationPostGramException("Con not modify subscription of another user");

            if (subscription.SlaveId == currentUserId && model.Status)
                throw new AuthorizationPostGramException(
                    "Can not confirm subscription being slave to this subscription");

            subscription.Status = model.Status;
            subscription.Edited = DateTimeOffset.UtcNow;

            try
            {
                await _dataContext.SaveChangesAsync();
                return _mapper.Map<SubscriptionModel>(subscription);
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException != null)
                {
                    throw new DbPostGramException(e.InnerException.Message, e.InnerException);
                }
                throw new DbPostGramException(e.Message, e);
            }
        }

        public async Task<UserModel> UpdateUser(UpdateUserModel model, Guid currentUserId)
        {
            User user = await GetUserById(model.UserId);

            if (user.Id != currentUserId)
                throw new AuthorizationPostGramException("Cannot modify another user");
            if (model.NewBirthDate != null)
                user.BirthDate = model.NewBirthDate.Value;
            if (model.NewIsPrivate != null)
                user.IsPrivate = model.NewIsPrivate.Value;
            if (model.NewName != null)
                user.Name = model.NewName;
            if (model.NewNickname != null)
                user.Nickname = model.NewNickname;
            if (model.NewPatronymic != null)
                user.Patronymic = model.NewPatronymic;
            if (model.NewSurname != null)
                user.Surname = model.NewSurname;

            try
            {
                //await _dataContext.Users.AddAsync(user);
                await _dataContext.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException != null)
                {
                    throw new DbPostGramException(e.InnerException.Message, e.InnerException);
                }
                throw new DbPostGramException(e.Message, e);
            }

            return _mapper.Map<UserModel>(user);
        }

        private async Task<User> GetUserById(Guid id)
        {
            User? user = await _dataContext.Users
                .Include(u => u.Avatar)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                throw new NotFoundPostGramException("User not found, userId: " + id);
            return user;
        }
    }
}
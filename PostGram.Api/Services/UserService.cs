using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using PostGram.Api.Models.Attachment;
using PostGram.Api.Models.User;
using PostGram.Common.Exceptions;
using PostGram.DAL;
using PostGram.DAL.Entities;

namespace PostGram.Api.Services
{
    public class UserService : IUserService, IDisposable
    {
        private readonly IMapper _mapper;
        private readonly DataContext _dataContext;

        public UserService(IMapper mapper, DataContext dataContext)
        {
            _mapper = mapper;
            _dataContext = dataContext;
        }

        public async Task<bool> CheckUserExist(string email)
        {
            return await _dataContext.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<Guid> CreateUser(CreateUserModel model)
        {
            if (await CheckUserExist(model.Email))
                throw new DbPostGramException("User with email already exist, email: " + model.Email);

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
        //TODO 1 To Test
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

        public async Task<UserModel> GetUser(Guid UserId)
        {
            User user = await GetUserById(UserId);
            return _mapper.Map<UserModel>(user);
        }

        public async Task AddAvatarToUser(Guid userId, MetadataModel model, string filePath)
        {
            User user = await GetUserById(userId);
            Avatar avatar = new Avatar()
            {
                Author = user,
                User = user,
                Id = model.TempId,
                MimeType = model.MimeType,
                Name = model.Name,
                Size = model.Size,
                FilePath = filePath
            };
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

        //TODO 1 To Test
        public async Task DeleteAvatarForUser(Guid userId)
        {
            User user = await GetUserById(userId);
            if (user.Avatar == null)
                throw new NotFoundPostGramException($"User {userId} does't have avatar");

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
        }

        public void Dispose()
        {
            _dataContext.Dispose();
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
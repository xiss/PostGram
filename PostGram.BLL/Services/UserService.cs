using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using PostGram.BLL.Interfaces.Services;
using PostGram.Common.Dtos.Attachment;
using PostGram.Common.Dtos.Like;
using PostGram.Common.Dtos.Post;
using PostGram.Common.Dtos.User;
using PostGram.Common.Exceptions;
using PostGram.Common.Requests.Commands;
using PostGram.DAL;
using PostGram.DAL.Entities;

namespace PostGram.BLL.Services
{
    public class UserService
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
                Created = DateTimeOffset.UtcNow
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
                    throw new PostGramException(e.InnerException.Message, e.InnerException);
                }
                throw new PostGramException(e.Message, e);
            }
        }

        public async Task<bool> CheckUserExist(string email)
        {
            return await _dataContext.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task CreateUser(CreateUserCommand model)
        {
            if (await CheckUserExist(model.Email))
                throw new UnprocessableRequestPostGramException("User with email already exist, email: " + model.Email);
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
                    throw new PostGramException(e.InnerException.Message, e.InnerException);
                }
                throw new PostGramException(e.Message, e);
            }
        }

        public async Task DeleteAvatarForUser(Guid userId)
        {
            User user = await GetUserById(userId);
            if (user.Avatar == null)
                throw new NotFoundPostGramException($"User {userId} does't have avatar.");
            try
            {
                _dataContext.Avatars.Remove(user.Avatar);
                await _dataContext.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException != null)
                {
                    throw new PostGramException(e.InnerException.Message, e.InnerException);
                }
                throw new PostGramException(e.Message, e);
            }
        }

        public async Task DeleteUser(Guid userId)
        {
            User user = await GetUserById(userId);
            if (user == null)
                throw new NotFoundPostGramException($"User {userId} not found");
            user.IsDelete = true;
            try
            {
                await _dataContext.UserSessions.Where(us => us.UserId == userId).ForEachAsync(us => us.IsActive = false);
                await _dataContext.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException != null)
                {
                    throw new PostGramException(e.InnerException.Message, e.InnerException);
                }
                throw new PostGramException(e.Message, e);
            }
        }

        

      

        public async Task<UserDto> GetUser(Guid userId)
        {
            User user = await GetUserById(userId);
            return _mapper.Map<UserDto>(user);
        }

        public async Task<List<UserDto>> GetUsers()
        {
            List<UserDto> models = await _dataContext
                .Users
                .Include(x => x.Avatar)
                .AsNoTracking()
                .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            if (models.Count == 0)
                throw new NotFoundPostGramException("Users not found in DB");
            return models;
        }

        public async Task UpdateUser(UpdateUserCommand model, Guid currentUserId)
        {
            User user = await GetUserById(model.UserId);

            //TODO Куда убрать валидацию?
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

            //TODO Убрать отсюда эти обработки ошибок
            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException != null)
                {
                    throw new PostGramException(e.InnerException.Message, e.InnerException);
                }
                throw new PostGramException(e.Message, e);
            }
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
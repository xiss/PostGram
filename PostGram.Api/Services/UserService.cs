using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PostGram.Api.Configs;
using PostGram.Api.Models;
using PostGram.Common;
using PostGram.Common.Exceptions;
using PostGram.DAL;
using PostGram.DAL.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PostGram.Api.Services
{
    public class UserService : IUserService, IDisposable
    {
        private readonly IMapper _mapper;
        private readonly DataContext _dataContext;
        private readonly AuthConfig _authConfig;

        public UserService(IMapper mapper, DataContext dataContext, IOptions<AuthConfig> config)
        {
            _mapper = mapper;
            _dataContext = dataContext;
            _authConfig = config.Value;
        }

        public async Task<bool> CheckUserExist(string email)
        {
            return await _dataContext.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<Guid> CreateUser(CreateUserModel model)
        {
            if (await CheckUserExist(model.Email))
                throw new DBCreatePostGramException("User with email already exist, email: " + model.Email);

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
                    throw new DBCreatePostGramException(e.InnerException.Message);
                }
                throw new DBPostGramException(e.Message);
            }

            return user.Id;
        }

        public async Task<Guid> DeleteUser(Guid userId)
        {
            User user = await GetUserById(userId);

            try
            {
                _dataContext.Users.Remove(user);
                await _dataContext.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException != null)
                {
                    throw new DBCreatePostGramException(e.InnerException.Message);
                }
                throw new DBPostGramException(e.Message);
            }

            return userId;
        }

        public async Task<List<UserModel>> GetUsers()
        {
            return await _dataContext.Users.AsNoTracking().ProjectTo<UserModel>(_mapper.ConfigurationProvider).ToListAsync();
        }

        private async Task<User> GetUserByCredential(string login, string password)
        {
            User? user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == login.ToLower());

            if (user == null)
                throw new UserNotFoundPostGramException("login: " + login);

            if (!HashHelper.Verify(password, user.PasswordHash))
                throw new AuthorizationPostGramException("Password incorrect for login: " + login);

            return user;
        }

        private async Task<User> GetUserById(Guid id)
        {
            User? user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                throw new UserNotFoundPostGramException("User not found, id: " + id);

            return user;
        }

        public async Task<UserModel> GetUser(Guid id)
        {
            User user = await GetUserById(id);
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
                    throw new DBCreatePostGramException(e.InnerException.Message);
                }
                throw new DBPostGramException(e.Message);
            }
        }

        private TokenModel GenerateTokens(UserSession session)
        {
            DateTime now = DateTime.Now;

            //SecurityToken
            Claim[] claims =
            {
                new(ClaimsIdentity.DefaultNameClaimType,  session.User.Name),
                new(Constants.ClaimTypeLogin , session.User.Login),
                new(Constants.ClaimTypeUserId, session.User.Id.ToString()),
                new(Constants.ClaimTypeSessionId, session.Id.ToString())
            };
            JwtSecurityToken securityToken = new JwtSecurityToken(
                issuer: _authConfig.Issuer,
                audience: _authConfig.Audience,
                notBefore: now,
                claims: claims,
                expires: now.AddMinutes(_authConfig.LifeTime),
                signingCredentials: new SigningCredentials(_authConfig.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256));
            string encodedToken = new JwtSecurityTokenHandler().WriteToken(securityToken);

            //RefreshToken
            claims = new Claim[]
            {
                new(Constants.ClaimTypeRefreshTokenId, session.RefreshTokenId.ToString()),
            };
            JwtSecurityToken refreshToken = new JwtSecurityToken(
                notBefore: now,
                claims: claims,
                expires: now.AddHours(_authConfig.LifeTime),
                signingCredentials: new SigningCredentials(_authConfig.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256));
            string encodedRefreshToken = new JwtSecurityTokenHandler().WriteToken(refreshToken);

            return new TokenModel(encodedToken, encodedRefreshToken);
        }

        public async Task<TokenModel> GetToken(string login, string password)
        {
            User user = await GetUserByCredential(login, password);

            var session = await _dataContext.UserSessions.AddAsync(new()
            {
                Id = Guid.NewGuid(),
                Created = DateTimeOffset.UtcNow,
                RefreshTokenId = Guid.NewGuid(),
                User = user
            });
            await _dataContext.SaveChangesAsync();
            return GenerateTokens(session.Entity);
        }

        public async Task<UserSession> GetUserSessionById(Guid id)
        {
            UserSession? session = await _dataContext.UserSessions.FirstOrDefaultAsync(s => s.Id == id);
            if (session == null)
                throw new SessionNotFoundPostGramException("Session with Id " + id + " not found");

            return session;
        }

        private async Task<UserSession> GetUserSessionByRefreshToken(Guid refreshTokenId)
        {
            UserSession? session = await _dataContext.UserSessions
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.RefreshTokenId == refreshTokenId);
            if (session == null)
                throw new SessionNotFoundPostGramException("Session with refreshTokenId " + refreshTokenId + " not found");

            return session;
        }

        public async Task<TokenModel> GetTokenByRefreshToken(string refreshToken)
        {
            TokenValidationParameters validationParameters = new()
            {
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                ValidateAudience = false,
                ValidateLifetime = true,
                IssuerSigningKey = _authConfig.GetSymmetricSecurityKey()
            };
            var principal = new JwtSecurityTokenHandler().ValidateToken(refreshToken, validationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken
                || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                //TODO почему если выкинуть здесь исключение и обработать в контроллере он выдает заголовок 500 и следующее:
                //System.InvalidOperationException: No authentication handler is registered for the scheme 'Invalid security token'.
                //The registered schemes are: Bearer.Did you forget to call AddAuthentication().Add[SomeAuthHandler]("Invalid security token", ...) ?
                throw new SecurityTokenPostGramException("Invalid security token");
            }

            if (principal.Claims.FirstOrDefault((c => c.Type == Constants.ClaimTypeRefreshTokenId))?.Value is String refreshIdString
                && Guid.TryParse(refreshIdString, out var refreshTokenId))
            {
                UserSession session = await GetUserSessionByRefreshToken(refreshTokenId);
                if (!session.IsActive)
                    throw new SessionIsInactivePostGramException("Session is inactive");

                User user = session.User;

                session.RefreshTokenId = Guid.NewGuid();
                await _dataContext.SaveChangesAsync();
                return GenerateTokens(session);
            }
            else
            {
                throw new SecurityTokenPostGramException("Invalid refresh token");
            }
        }

        public void Dispose()
        {
            _dataContext.Dispose();
        }
    }
}
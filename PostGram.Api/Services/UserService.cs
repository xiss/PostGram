using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PostGram.Api.Configs;
using PostGram.Api.Models;
using PostGram.Common;
using PostGram.Common.Exceptions;
using PostGram.DAL.Entities;
using PostGram.DAL;

namespace PostGram.Api.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _dataContext;
        private readonly AuthConfig _authConfig;
        private const string ClaimTypeLogin = "login";
        private const string ClaimTypeId = "id";


        public UserService(IMapper mapper, DataContext dataContext, IOptions<AuthConfig> config)
        {
            _mapper = mapper;
            _dataContext = dataContext;
            _authConfig = config.Value;
        }

        [HttpPost]
        public async Task CreateUser(CreateUserModel model)
        {
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
                    throw new DBCreatePostGramException(e.InnerException. Message);
                }
                throw new DBPostGramException(e.Message);
            }
        }

        [HttpGet]
        public async Task<List<UserModel>> GetUsers()
        {
            return await _dataContext.Users.AsNoTracking().ProjectTo<UserModel>(_mapper.ConfigurationProvider).ToListAsync();
        }

        private async Task<User> GetUserByCredential(string login, string password)
        {
            User? user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Login.ToLower() == login.ToLower());

            if (user == null)
                throw new UserNotFoundPostGramException("login: " + login);

            if (!HashHelper.Verify(password, user.PasswordHash))
                throw new AuthorizationPostGramException("login: " + login);

            return user;
        }

        private async Task<User> GetUserById(Guid id)
        {
            User? user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                throw new UserNotFoundPostGramException("id: " + id);
            return user;
        }

        public async Task<UserModel> GetUser(Guid id)
        {
            User user = await GetUserById(id);
            return _mapper.Map<UserModel>(user);
        }

        private TokenModel GenerateToken(User user)
        {
            DateTime now = DateTime.Now;

            //SecurityToken
            Claim[] claims =
            {
                new(ClaimsIdentity.DefaultNameClaimType, user.Name),
                new(ClaimTypeLogin , user.Login),
                new(ClaimTypeId, user.Id.ToString())
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
                new(nameof(user.Id), user.Id.ToString())
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
            return GenerateToken(user);
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
                || !jwtSecurityToken.Header.Alg.Equals((SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new SecurityTokenPostGramException("Invalid security token");
            }

            if (principal.Claims.FirstOrDefault(c => c.Type == nameof(User))?.Value is String userIdString
                && Guid.TryParse(userIdString, out var userId))
            {
                User user = await GetUserById(userId);
                return GenerateToken(user);
            }
            else
            {
                throw new SecurityTokenPostGramException("Invalid refresh token");
            }
        }
    }
}

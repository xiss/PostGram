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
            await _dataContext.Users.AddAsync(user);
            await _dataContext.SaveChangesAsync();
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
                throw new UserNotFoundException(login);

            if (!HashHelper.Verify(password, user.PasswordHash))
                throw new AuthorizationException(login);

            return user;
        }

        public async Task<UserModel> GetUser(Guid id)
        {
            User? user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id== id);
            if (user == null)
                throw new UserNotFoundException(id: id);
            return _mapper.Map<UserModel>(user);
        }

        public async Task<TokenModel> GetToken(string login, string password)
        {
            User user = await GetUserByCredential(login, password);

            Claim[] claims =
            {
                new(nameof(user.Name), user.Name),
                new(nameof(user.Login), user.Login),
                new(nameof(user.Id), user.Id.ToString())
            };

            DateTime now = DateTime.Now;
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _authConfig.Issuer,
                audience: _authConfig.Audience,
                notBefore: now,
                claims: claims,
                expires: now.AddMinutes(_authConfig.LifeTime),
                signingCredentials: new SigningCredentials(_authConfig.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256));
            string encodedToken = new JwtSecurityTokenHandler().WriteToken(token);

            return new TokenModel(encodedToken);
        }
    }
}

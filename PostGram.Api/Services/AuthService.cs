﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PostGram.Api.Configs;
using PostGram.Api.Models.Token;
using PostGram.Common;
using PostGram.Common.Constants;
using PostGram.Common.Exceptions;
using PostGram.DAL;
using PostGram.DAL.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PostGram.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly DataContext _dataContext;
        private readonly AuthConfig _authConfig;

        public AuthService(DataContext dataContext, IOptions<AuthConfig> authConfig)
        {
            _dataContext = dataContext;
            _authConfig = authConfig.Value;
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
                throw new NotFoundPostGramException("Session with Id " + id + " not found");

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
                //TODO почему если выкинуть здесь исключение и обработать в контроллере он выдает заголовок 500 и следующее, а не то что мы отдаем в контроллере:
                ///throw new AuthorizationPostGramException("test");
                //System.InvalidOperationException: No authentication handler is registered for the scheme 'Invalid security token'.
                //The registered schemes are: Bearer.Did you forget to call AddAuthentication().Add[SomeAuthHandler]("Invalid security token", ...) ?
                throw new AuthorizationPostGramException("Invalid security token");
            }

            if (principal.Claims.FirstOrDefault((c => c.Type == ClaimNames.RefreshTokenId))?.Value is String refreshIdString
                && Guid.TryParse(refreshIdString, out var refreshTokenId))
            {
                UserSession session = await GetUserSessionByRefreshToken(refreshTokenId);
                if (!session.IsActive)
                    throw new AuthorizationPostGramException("Session is inactive");

                User user = session.User;

                session.RefreshTokenId = Guid.NewGuid();
                await _dataContext.SaveChangesAsync();
                return GenerateTokens(session);
            }
            else
            {
                throw new AuthorizationPostGramException("Invalid refresh token");
            }
        }

        private async Task<User> GetUserByCredential(string login, string password)
        {
            User? user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == login.ToLower());

            if (user == null)
                throw new NotFoundPostGramException("login: " + login);

            if (!HashHelper.Verify(password, user.PasswordHash))
                throw new AuthorizationPostGramException("Password incorrect for login: " + login);

            return user;
        }

        private TokenModel GenerateTokens(UserSession session)
        {
            DateTime now = DateTime.Now;

            //SecurityToken
            Claim[] claims =
            {
                new(ClaimsIdentity.DefaultNameClaimType,  session.User.Name),
                new(ClaimNames.Login , session.User.Login),
                new(ClaimNames.UserId, session.User.Id.ToString()),
                new(ClaimNames.SessionId, session.Id.ToString())
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
                new(ClaimNames.RefreshTokenId, session.RefreshTokenId.ToString()),
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

        private async Task<UserSession> GetUserSessionByRefreshToken(Guid refreshTokenId)
        {
            UserSession? session = await _dataContext.UserSessions
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.RefreshTokenId == refreshTokenId);
            if (session == null)
                throw new NotFoundPostGramException("Session with refreshTokenId " + refreshTokenId + " not found");

            return session;
        }
    }
}
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PostGram.Common;
using PostGram.Common.Configs;
using PostGram.Common.Constants;
using PostGram.Common.Exceptions;
using PostGram.DAL;
using PostGram.DAL.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using PostGram.BLL.Interfaces.Services;
using PostGram.Common.Dtos;

namespace PostGram.BLL.Services;

public class TokenService : ITokenService
{
    private readonly AuthConfig _authConfig;
    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;
    private readonly TimeProvider _timeProvider;

    public TokenService(DataContext dataContext, AuthConfig authConfig, IMapper mapper, TimeProvider timeProvider)
    {
        _dataContext = dataContext;
        _authConfig = authConfig;
        _mapper = mapper;
        _timeProvider = timeProvider;
    }

    //TODO Метод точно должен быть тут и быть статичным?
    public static SymmetricSecurityKey GetSymmetricSecurityKey(string key)
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    }

    public async Task<TokenDto> GetToken(string login, string password)
    {
        User user = await GetUserByCredential(login, password);

        var session = await _dataContext.UserSessions.AddAsync(new()
        {
            Id = Guid.NewGuid(),
            Created = _timeProvider.GetUtcNow(),
            RefreshTokenId = Guid.NewGuid(),
            User = user
        });
        await _dataContext.SaveChangesAsync();
        return GenerateTokens(session.Entity);
    }

    public async Task<TokenDto> GetTokenByRefreshToken(string refreshToken)
    {
        TokenValidationParameters validationParameters = new()
        {
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            IssuerSigningKey = GetSymmetricSecurityKey(_authConfig.Key)
        };
        var principal = new JwtSecurityTokenHandler().ValidateToken(refreshToken, validationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken
            || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
        {
            throw new AuthorizationPostGramException("Invalid security token");
        }

        if (Enumerable.FirstOrDefault<Claim>(principal.Claims, c => c.Type == ClaimNames.RefreshTokenId)?.Value is string refreshIdString
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

    public async Task<UserSessionDto> GetUserSessionById(Guid id)
    {
        UserSession? session = await _dataContext.UserSessions.FirstOrDefaultAsync(s => s.Id == id);
        if (session == null)
            throw new NotFoundPostGramException("Session with Id " + id + " not found");

        return _mapper.Map<UserSessionDto>(session);
    }

    private TokenDto GenerateTokens(UserSession session)
    {
        DateTime now = _timeProvider.GetUtcNow().DateTime;

        //SecurityToken
        Claim[] claims =
        {
            new(ClaimsIdentity.DefaultNameClaimType,  session.User.Name),
            new(ClaimNames.Login , session.User.Nickname),
            new(ClaimNames.UserId, session.User.Id.ToString()),
            new(ClaimNames.SessionId, session.Id.ToString())
        };
        JwtSecurityToken securityToken = new JwtSecurityToken(
            issuer: _authConfig.Issuer,
            audience: _authConfig.Audience,
            notBefore: now,
            claims: claims,
            expires: now.AddMinutes(_authConfig.LifeTime),
            signingCredentials: new SigningCredentials(GetSymmetricSecurityKey(_authConfig.Key),
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
            signingCredentials: new SigningCredentials(GetSymmetricSecurityKey(_authConfig.Key),
                SecurityAlgorithms.HmacSha256));
        string encodedRefreshToken = new JwtSecurityTokenHandler().WriteToken(refreshToken);

        return new TokenDto() { RefreshToken = encodedRefreshToken, SecurityToken = encodedToken };
    }

    private async Task<User> GetUserByCredential(string login, string password)
    {
        User? user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == login.ToLower());

        if (user == null)
            throw new NotFoundPostGramException("login: " + login);

        if (!HashHelper.VerifySha256(password, user.PasswordHash))
            throw new AuthorizationPostGramException("Password incorrect for login: " + login);

        return user;
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


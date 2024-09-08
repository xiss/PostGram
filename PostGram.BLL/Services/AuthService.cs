using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PostGram.Common;
using PostGram.Common.Configs;
using PostGram.Common.Constants;
using PostGram.Common.Dtos.Token;
using PostGram.Common.Dtos.User;
using PostGram.Common.Exceptions;
using PostGram.Common.Interfaces.Services;
using PostGram.DAL;
using PostGram.DAL.Entities;

namespace PostGram.BLL.Services;

public class AuthService : IDisposable, IAuthService
{
    private readonly AuthConfig _authConfig;
    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;

    public AuthService(DataContext dataContext, IOptions<AuthConfig> authConfig, IMapper mapper)
    {
        _dataContext = dataContext;
        _authConfig = authConfig.Value;
        _mapper = mapper;
    }

    public void Dispose()
    {
        _dataContext.Dispose();
    }

    public async Task<TokenDto> GetToken(string login, string password)
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

    public async Task Logout(Guid userId, Guid sessionId)
    {
        UserSession? session = await _dataContext.UserSessions
            .FirstOrDefaultAsync(us => us.Id == sessionId);
        if (session == null)
            throw new NotFoundPostGramException($"Session: {sessionId} for user: {userId} not found");
        session.IsActive = false;

        try
        {
            _dataContext.UserSessions.Update(session);
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

    private TokenDto GenerateTokens(UserSession session)
    {
        DateTime now = DateTime.Now;

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

    //TODO Метод точно должен быть тут и быть статичным?
    public static SymmetricSecurityKey GetSymmetricSecurityKey(string key)
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    }
}
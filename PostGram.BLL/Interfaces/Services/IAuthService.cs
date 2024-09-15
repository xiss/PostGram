﻿using PostGram.Common.Dtos;

namespace PostGram.BLL.Interfaces.Services;

public interface IAuthService
{
    Task<TokenDto> GetToken(string login, string password);

    Task<TokenDto> GetTokenByRefreshToken(string refreshToken);

    Task<UserSessionDto> GetUserSessionById(Guid id);

    Task Logout(Guid userId, Guid sessionId);
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostGram.Api.Helpers;
using PostGram.Common.Dtos.Token;
using PostGram.Common.Interfaces.Services;
using PostGram.Common.Requests;

namespace PostGram.Api.Controllers;

[ApiExplorerSettings(GroupName = Common.Constants.Api.EndpointAuthorizationName)]
[Route("api/[controller]/[action]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;
    public AuthController(IUserService userService, IAuthService authService)
    {
        _userService = userService;
        _authService = authService;
    }

    [Authorize]
    [HttpPost]
    public async Task Logout()
    {
        await _authService.Logout(this.GetCurrentUserId(), this.GetCurrentSessionId());
    }

    [HttpPost]
    public async Task<TokenDto> RefreshToken(RefreshTokenRequestModel model)
    {
        return await _authService.GetTokenByRefreshToken(model.RefreshToken); 
    }

    [HttpPost]
    public async Task RegisterUser(CreateUserModel model)
    {
        await _userService.CreateUser(model);
    }

    [HttpPost]
    public async Task<TokenDto> Token(TokenRequestModel model)
    {
        return await _authService.GetToken(model.Login, model.Password); 
    }
}
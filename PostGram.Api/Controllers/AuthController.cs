using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostGram.Api.Helpers;
using PostGram.BLL.Interfaces.Services;
using PostGram.Common.Dtos;
using PostGram.Common.Requests;
using PostGram.Common.Requests.Commands;

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
    public async Task<TokenDto> Token(TokenRequestModel model)
    {
        return await _authService.GetToken(model.Login, model.Password);
    }
}
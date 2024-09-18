using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostGram.BLL.Interfaces.Services;
using PostGram.Common.Dtos;
using PostGram.Common.Interfaces.Base.Commands;
using PostGram.Common.Requests;
using PostGram.Common.Requests.Commands;

namespace PostGram.Api.Controllers;

[ApiExplorerSettings(GroupName = Common.Constants.Api.EndpointAuthorizationName)]
[Route("api/[controller]/[action]")]
[ApiController]
public class TokenController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly ICommandHandler<LogoutCommand> _logout;

    public TokenController(ITokenService authService, 
        ICommandHandler<LogoutCommand> logout)
    {
        _tokenService = authService;
        _logout = logout;
    }

    [Authorize]
    [HttpPost]
    public async Task Logout(LogoutCommand command)
    {
        await _logout.Execute(command);
    }

    [HttpPost]
    public async Task<TokenDto> RefreshToken(RefreshTokenRequestModel model)
    {
        return await _tokenService.GetTokenByRefreshToken(model.RefreshToken);
    }

    [HttpPost]
    public async Task<TokenDto> Token(TokenRequestModel model)
    {
        return await _tokenService.GetToken(model.Login, model.Password);
    }
}
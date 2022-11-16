using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostGram.Api.Models.Token;
using PostGram.Api.Models.User;
using PostGram.Api.Services;
using PostGram.Api.Helpers;

namespace PostGram.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;

        public AuthController(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        [HttpPost]
        public async Task RegisterUser(CreateUserModel model)
        {
            await _userService.CreateUser(model);
        }

        [HttpPost]
        public async Task<TokenModel> Token(TokenRequestModel model)
        {
            return await _authService.GetToken(model.Login, model.Password);
        }

        [HttpPost]
        public async Task<TokenModel> RefreshToken(RefreshTokenRequestModel model)
        {
            return await _authService.GetTokenByRefreshToken(model.RefreshToken);
        }

        [Authorize]
        [HttpPost]
        public async Task Logout()
        {
            {
                await _authService.Logout(this.GetCurrentUserId(), this.GetCurrentSessionId());
            }
        }
    }
}
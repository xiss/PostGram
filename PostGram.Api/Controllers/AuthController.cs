using Microsoft.AspNetCore.Mvc;
using PostGram.Api.Models.Token;
using PostGram.Api.Models.User;
using PostGram.Api.Services;
using PostGram.Common.Exceptions;
using LogLevel = NLog.LogLevel;

namespace PostGram.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly NLog.Logger _logger;

        public AuthController(IUserService userService, IAuthService authService)
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
            _userService = userService;
            _authService = authService;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser(CreateUserModel model)
        {
            try
            {
                await _userService.CreateUser(model);
            }
            catch (DbPostGramException e)
            {
                _logger.Log(LogLevel.Warn, e);
                return StatusCode(500, e.Message);
            }

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult<TokenModel>> Token(TokenRequestModel model)
        {
            try
            {
                return await _authService.GetToken(model.Login, model.Password);
            }
            catch (NotFoundPostGramException e)
            {
                _logger.Log(LogLevel.Warn, e);
                return NotFound(e.Message);
            }
            catch (AuthorizationPostGramException e)
            {
                _logger.Log(LogLevel.Warn, e);
                return Unauthorized(e.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<TokenModel>> RefreshToken(RefreshTokenRequestModel model)
        {
            try
            {
                return await _authService.GetTokenByRefreshToken(model.RefreshToken);
            }
            catch (AuthorizationPostGramException e)
            {
                _logger.Log(LogLevel.Warn, e);
                return Unauthorized();
            }
        }
    }
}
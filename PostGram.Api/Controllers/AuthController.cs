using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PostGram.Api.Models;
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
        private readonly NLog.Logger _logger;

        public AuthController(IUserService userService)
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
            _userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult<TokenModel>> Token(TokenRequestModel model)
        {
            try
            {
                return await _userService.GetToken(model.Login, model.Password);
            }
            catch (UserNotFoundPostGramException e)
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
                return await _userService.GetTokenByRefreshToken(model.RefreshToken);
            }
            catch (AuthorizationPostGramException e)
            {
                _logger.Log(LogLevel.Warn, e);
                return Forbid(e.Message);
            }           
        }
    }
}

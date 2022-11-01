using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PostGram.Api.Models;
using PostGram.Api.Services;
using PostGram.Common.Exceptions;

namespace PostGram.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
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
                return NotFound(e.Message);
            }
            catch (AuthorizationPostGramException e)
            {
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
                return Forbid(e.Message);
            }           
        }
    }
}

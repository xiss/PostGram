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
            catch (UserNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (AuthorizationException e)
            {
                return Unauthorized(e.Message);
            }
            catch (Exception e)
            {
                //TODO Логирование
                Console.WriteLine(e);
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<TokenModel>> RefreshToken(RefreshTokenRequestModel model)
        {
            try
            {
                return await _userService.GetTokenByRefreshToken(model.RefreshToken);
            }
            catch (SecurityTokenException e)
            {
                return Forbid(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e.Message);
            }
        }
    }
}

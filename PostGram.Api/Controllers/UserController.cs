using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PostGram.Api.Configs;
using PostGram.Api.Models;
using PostGram.Api.Services;
using PostGram.Common;
using PostGram.Common.Exceptions;
using LogLevel = NLog.LogLevel;

namespace PostGram.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAttachService _attachService;
        private readonly NLog.Logger _logger;
        private readonly AppConfig _appConfig;

        public UserController(IUserService userService, IOptions<AppConfig> config, IAttachService attachService)
        {
            _userService = userService;
            _logger = NLog.LogManager.GetCurrentClassLogger();
            _appConfig = config.Value;
            _attachService = attachService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserModel model)
        {
            try
            {
                await _userService.CreateUser(model);
            }
            catch (DBCreatePostGramException e)
            {
                _logger.Log(LogLevel.Warn, e);
                return StatusCode(500, e.Message);
            }
            
            return Ok();
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<UserModel>>> GetUsers()
        {
            List<UserModel> users = new();
            try
            {
                users = await _userService.GetUsers();
            }
            catch (AuthorizationPostGramException e)
            {
                _logger.Log(LogLevel.Warn, e);
                return Forbid(e.Message);
            }

            return Ok(users);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<UserModel>> GetCurrentUser()
        {
            try
            {
                return await _userService.GetUser(GetCurrentUserId());
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
        [Authorize]
        public async Task<ActionResult> AddAvatarToUser(MetadataModel model)
        {
            try
            {
                string destFile = await _attachService.MoveToAttaches(model.TempId.ToString());
                await _userService.AddAvatarToUser(GetCurrentUserId(), model, destFile);
                return Ok();
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
            catch (AttachPostGramException e)
            {
                _logger.Log(LogLevel.Error, e);
                if (e.InnerException != null)
                    return StatusCode(500, e.InnerException.Message);

                return StatusCode(500, e.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetAvatarForUser(Guid userId)
        {
            try
            {
                AttachModel model = await _attachService.GetAvatarForUser(userId);
                return File(await System.IO.File.ReadAllBytesAsync(model.FilePath), model.MimeType);
            }
            catch (AttachPostGramException e)
            {
                _logger.Log(LogLevel.Error, e);
                return  StatusCode(500, e.Message);
            }
        }

        private Guid GetCurrentUserId()
        {
            string? userIdStr = User.Claims.FirstOrDefault(c => c.Type == Constants.ClaimTypeUserId)?.Value;
            if (Guid.TryParse(userIdStr, out var userId))
                return userId;

            throw new AuthorizationPostGramException("You are not authorized");
        }

        //public async Task RefreshPassword()
        //{
        //    //TODO RefreshPassword
        //    throw new NotImplementedException();
        //}
        //public async Task RefreshLogin()
        //{
        //    //TODO RefreshLogin
        //    throw new NotImplementedException();
        //}
    }
}

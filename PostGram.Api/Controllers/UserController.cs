using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PostGram.Api.Configs;
using PostGram.Api.Models;
using PostGram.Api.Services;
using PostGram.Common.Exceptions;
using LogLevel = NLog.LogLevel;

namespace PostGram.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAttachmentService _attachmentService;
        private readonly NLog.Logger _logger;
        private readonly AppConfig _appConfig;

        public UserController(IUserService userService, IOptions<AppConfig> config, IAttachmentService attachmentService)
        {
            _userService = userService;
            _logger = NLog.LogManager.GetCurrentClassLogger();
            _appConfig = config.Value;
            _attachmentService = attachmentService;
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
                return await _userService.GetUser(this.GetCurrentUserId());
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
                string destFile = await _attachmentService.ApplyFile(model.TempId.ToString());
                await _userService.AddAvatarToUser(this.GetCurrentUserId(), model, destFile);
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
                AttachmentModel model = await _attachmentService.GetAvatarForUser(userId);
                return File(await System.IO.File.ReadAllBytesAsync(model.FilePath), model.MimeType);
            }
            catch (AttachPostGramException e)
            {
                _logger.Log(LogLevel.Error, e);
                return StatusCode(500, e.Message);
            }
        }

        //public async Task RefreshPassword()
        //{
        //    //TODO 3 RefreshPassword
        //    throw new NotImplementedException();
        //}
        //public async Task RefreshLogin()
        //{
        //    //TODO 3 RefreshLogin
        //    throw new NotImplementedException();
        //}
    }
}
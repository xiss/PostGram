using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostGram.Api.Helpers;
using PostGram.Api.Models.Attachment;
using PostGram.Api.Models.User;
using PostGram.Api.Services;
using PostGram.Common.Exceptions;
using LogLevel = NLog.LogLevel;

namespace PostGram.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    //[Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAttachmentService _attachmentService;
        private readonly NLog.Logger _logger;

        public UserController(IUserService userService, IAttachmentService attachmentService)
        {
            _userService = userService;
            _logger = NLog.LogManager.GetCurrentClassLogger();
            _attachmentService = attachmentService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserModel>>> GetUsers()
        {
            try
            {
                List<UserModel> models = await _userService.GetUsers();
                foreach (UserModel user in models)
                {
                    if (user.Avatar != null)
                        user.Avatar.Link = AttachmentController.GetLinkForAvatar(Url, user.Id);
                }

                return Ok(models);
            }
            catch (AuthorizationPostGramException e)
            {
                _logger.Log(LogLevel.Warn, e);
                return Unauthorized(e.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<UserModel>> GetCurrentUser()
        {
            try
            {
                UserModel model = await _userService.GetUser(this.GetCurrentUserId());
                model.Avatar.Link = AttachmentController.GetLinkForAvatar(Url, model.Id);
                return model;
            }
            catch (NotFoundPostGramException e)
            {
                _logger.Log(LogLevel.Warn, e);
                return NotFound(e.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddAvatarToUser(MetadataModel model)
        {
            try
            {
                string destFile = await _attachmentService.ApplyFile(model.TempId.ToString());
                await _userService.AddAvatarToUser(this.GetCurrentUserId(), model, destFile);
                return Ok();
            }
            catch (NotFoundPostGramException e)
            {
                _logger.Log(LogLevel.Warn, e);
                return NotFound(e.Message);
            }
            catch (FilePostGramException e)
            {
                _logger.Log(LogLevel.Error, e);
                if (e.InnerException != null)
                    return StatusCode(500, e.InnerException.Message);

                return StatusCode(500, e.Message);
            }
        }



        //public async Task<ActionResult> RefreshPassword()
        //{
        //    //TODO 3 RefreshPassword
        //    return StatusCode(501, "Not Implemented");
        //}

        //public async Task RefreshLogin()
        //{
        //    //TODO 3 RefreshLogin
        //    throw new NotImplementedException();
        //}
    }
}
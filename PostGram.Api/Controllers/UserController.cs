using Microsoft.AspNetCore.Mvc;
using PostGram.Api.Helpers;
using PostGram.Api.Models.Attachment;
using PostGram.Api.Models.User;
using PostGram.Api.Services;

namespace PostGram.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAttachmentService _attachmentService;
        private readonly IUserService _userService;

        public UserController(IUserService userService, IAttachmentService attachmentService)
        {
            _userService = userService;
            _attachmentService = attachmentService;
        }

        [HttpPost]
        public async Task AddAvatarToUser(MetadataModel model)
        {
            string destFile = await _attachmentService.ApplyFile(model.TempId.ToString());
            await _userService.AddAvatarToUser(this.GetCurrentUserId(), model, destFile);
        }

        [HttpDelete]
        public async Task<Guid> DeleteUserAvatar()
        {
            Guid avatarId = await _userService.DeleteAvatarForUser(this.GetCurrentUserId());
            _attachmentService.DeleteFile(avatarId);
            return avatarId;
        }

        [HttpGet]
        public async Task<UserModel> GetCurrentUser()
        {
            UserModel model = await _userService.GetUser(this.GetCurrentUserId());
            model.Avatar.Link = AttachmentController.GetLinkForAvatar(Url, model.Id);
            return model;
        }

        [HttpGet]
        public async Task<IEnumerable<UserModel>> GetUsers()
        {
            List<UserModel> models = await _userService.GetUsers();
            foreach (UserModel user in models)
            {
                if (user.Avatar != null)
                    user.Avatar.Link = AttachmentController.GetLinkForAvatar(Url, user.Id);
            }
            return models;
        }

        //public async Task<ActionResult> RefreshPassword()
        //{
        //    //TODO 4 RefreshPassword
        //    return StatusCode(501, "Not Implemented");
        //}

        //public async Task RefreshLogin()
        //{
        //    //TODO 4 RefreshLogin
        //    throw new NotImplementedException();
        //}
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostGram.Api.Helpers;
using PostGram.Api.Models.Attachment;
using PostGram.Api.Models.User;
using PostGram.Api.Services;
using PostGram.Common.Exceptions;

namespace PostGram.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    //[Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAttachmentService _attachmentService;

        public UserController(IUserService userService, IAttachmentService attachmentService)
        {
            _userService = userService;
            _attachmentService = attachmentService;
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

        [HttpGet]
        public async Task<UserModel> GetCurrentUser()
        {
            UserModel model = await _userService.GetUser(this.GetCurrentUserId());
            model.Avatar.Link = AttachmentController.GetLinkForAvatar(Url, model.Id);
            return model;
        }

        [HttpPost]
        public async Task AddAvatarToUser(MetadataModel model)
        {
            string destFile = await _attachmentService.ApplyFile(model.TempId.ToString());
            await _userService.AddAvatarToUser(this.GetCurrentUserId(), model, destFile);
        }

        //TODO 1 To test
        [HttpDelete]
        public async Task DeleteUserAvatar()
        {
            UserModel user = await _userService.GetUser(this.GetCurrentUserId());
            if (user.Avatar == null)
                throw new NotFoundPostGramException($"User {user.Id} dont have avatar");

            _attachmentService.DeleteFile(user.Avatar.Id);
            await _userService.DeleteAvatarForUser(user.Id);
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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostGram.Api.Helpers;
using PostGram.Common.Dtos.Attachment;
using PostGram.Common.Dtos.Subscription;
using PostGram.Common.Dtos.User;
using PostGram.Common.Interfaces.Services;
using PostGram.Common.Requests;

namespace PostGram.Api.Controllers;

[ApiExplorerSettings(GroupName = Common.Constants.Api.EndpointApiName)]
[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
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

    [HttpPost]
    public async Task<Guid> CreateSubscription(CreateSubscriptionModel model)
    {
        return await _userService.CreateSubscription(model, this.GetCurrentUserId());
    }

    [HttpDelete]
    public async Task<Guid> DeleteCurrentUser()
    {
        return await _userService.DeleteUser(this.GetCurrentUserId());
    }

    [HttpDelete]
    public async Task<Guid> DeleteCurrentUserAvatar()
    {
        Guid avatarId = await _userService.DeleteAvatarForUser(this.GetCurrentUserId());
        _attachmentService.DeleteFile(avatarId);
        return avatarId;
    }

    [HttpGet]
    public async Task<UserDto> GetCurrentUser()
    {
        UserDto model = await _userService.GetUser(this.GetCurrentUserId());
        //TODO
        //if (model.Avatar != null)
        //    model.Avatar.Link = AttachmentController.GetLinkForAvatar(Url, model.Id);
        return model;
    }

    [HttpGet]
    public async Task<List<SubscriptionDto>> GetMasterSubscriptions()
    {
        return await _userService.GetMasterSubscriptions(this.GetCurrentUserId());
    }

    [HttpGet]
    public async Task<List<SubscriptionDto>> GetSlaveSubscriptions()
    {
        return await _userService.GetSlaveSubscriptions(this.GetCurrentUserId());
    }

    [HttpGet]
    public async Task<IEnumerable<UserDto>> GetUsers()
    {
        List<UserDto> models = await _userService.GetUsers();
        //TODO
        //foreach (UserDto user in models)
        //{
        //    if (user.Avatar != null)
        //        user.Avatar.Link = AttachmentController.GetLinkForAvatar(Url, user.Id);
        //}
        return models;
    }

    [HttpPut]
    public async Task<SubscriptionDto> UpdateSubscription(UpdateSubscriptionModel model)
    {
        return await _userService.UpdateSubscription(model, this.GetCurrentUserId());
    }

    [HttpPut]
    public async Task<UserDto> UpdateUser(UpdateUserModel model)
    {
        return await _userService.UpdateUser(model, this.GetCurrentUserId());
    }
}
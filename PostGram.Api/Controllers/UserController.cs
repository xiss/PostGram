using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostGram.Api.Helpers;
using PostGram.Common.Dtos.Attachment;
using PostGram.Common.Dtos.Subscription;
using PostGram.Common.Dtos.User;
using PostGram.Common.Interfaces.Services;
using PostGram.Common.Requests.Commands;

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
    public async Task CreateSubscription(CreateSubscriptionCommand model)
    {
        await _userService.CreateSubscription(model, this.GetCurrentUserId());
    }

    [HttpDelete]
    public async Task DeleteCurrentUser()
    {
        await _userService.DeleteUser(this.GetCurrentUserId());
    }

    [HttpDelete]
    public async Task DeleteCurrentUserAvatar()
    {
        //TODO Починить
        //Guid avatarId = await _userService.DeleteAvatarForUser(this.GetCurrentUserId());
        //_attachmentService.DeleteFile(avatarId);
    }

    [HttpGet]
    public async Task<UserDto> GetCurrentUser()
    {
        return await _userService.GetUser(this.GetCurrentUserId());
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
        return await _userService.GetUsers();
    }

    [HttpPut]
    public async Task UpdateSubscription(UpdateSubscriptionCommand model)
    {
        await _userService.UpdateSubscription(model, this.GetCurrentUserId());
    }

    [HttpPut]
    public async Task TaskUpdateUser(UpdateUserCommand model)
    {
        await _userService.UpdateUser(model, this.GetCurrentUserId());
    }
}
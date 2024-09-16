using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostGram.Common.Interfaces.Base.Commands;
using PostGram.Common.Requests.Commands;

namespace PostGram.Api.Controllers;

[ApiExplorerSettings(GroupName = Common.Constants.Api.EndpointApiName)]
[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
public class AvatarController : ControllerBase
{
    private readonly ICommandHandler<AddAvatarToUserCommand> _addAvatarToUserHandler;
    private readonly ICommandHandler<DeleteCurrentUserAvatarCommand> _deleteCurrentUserAvatarHandlerHandler;

    public AvatarController(
        ICommandHandler<DeleteCurrentUserAvatarCommand> deleteCurrentUserAvatarHandlerHandler,
        ICommandHandler<AddAvatarToUserCommand> addAvatarToUserHandler)
    {
        _deleteCurrentUserAvatarHandlerHandler = deleteCurrentUserAvatarHandlerHandler            ;
        _addAvatarToUserHandler = addAvatarToUserHandler;
    }

    [HttpPost]
    public async Task AddAvatarToUser(AddAvatarToUserCommand command)
    {
        await _addAvatarToUserHandler.Execute(command);
    }

    [HttpDelete]
    public async Task DeleteCurrentUserAvatar(DeleteCurrentUserAvatarCommand command)
    {
        await _deleteCurrentUserAvatarHandlerHandler.Execute(command);
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostGram.Common.Interfaces.Base.Commands;
using PostGram.Common.Interfaces.Base.Queries;
using PostGram.Common.Requests.Commands;
using PostGram.Common.Requests.Queries;
using PostGram.Common.Results;

namespace PostGram.Api.Controllers;

[ApiExplorerSettings(GroupName = Common.Constants.Api.EndpointApiName)]
[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    private readonly ICommandHandler<CreateUserCommand> _createUserHandler;
    private readonly ICommandHandler<UpdateUserCommand> _updateUserHandler;
    private readonly ICommandHandler<DeleteCurrentUserCommand> _deleteCurrentUserHandler;
    private readonly IQueryHandler<GetUsersQuery, GetUsersResult> _getUsersHandler;
    private readonly IQueryHandler<GetCurrentUserQuery, GetCurrentUserResult> _getCurrentUserHandler;

    public UserController(
        ICommandHandler<CreateUserCommand> createUserHandler,
        ICommandHandler<UpdateUserCommand> updateUserHandler,
        ICommandHandler<DeleteCurrentUserCommand> deleteCurrentUserHandler,
        IQueryHandler<GetUsersQuery, GetUsersResult> getUsersHandler,
        IQueryHandler<GetCurrentUserQuery, GetCurrentUserResult> getCurrentUserHandler)
    {
        _createUserHandler = createUserHandler ;
        _updateUserHandler = updateUserHandler;
        _deleteCurrentUserHandler = deleteCurrentUserHandler;
        _getUsersHandler = getUsersHandler;
        _getCurrentUserHandler = getCurrentUserHandler;
    }

    [HttpDelete]
    public async Task DeleteCurrentUser(DeleteCurrentUserCommand command)
    {
        await _deleteCurrentUserHandler.Execute(command);
    }

    [HttpGet]
    public async Task<GetCurrentUserResult> GetCurrentUser(GetCurrentUserQuery query)
    {
        return await _getCurrentUserHandler.Execute(query);
    }

    [HttpGet]
    public async Task<GetUsersResult> GetUsers(GetUsersQuery query)
    {
        return await _getUsersHandler.Execute(query);
    }

    [HttpPut]
    public async Task TaskUpdateUser(UpdateUserCommand command)
    {
        await _updateUserHandler.Execute(command);
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task RegisterUser(CreateUserCommand command)
    {
        await _createUserHandler.Execute(command);
    }
}
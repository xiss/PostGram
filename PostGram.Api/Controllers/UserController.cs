using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostGram.BLL.Interfaces.Base.Commands;
using PostGram.BLL.Interfaces.Base.Queries;
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
        _createUserHandler = createUserHandler ?? throw new ArgumentNullException(nameof(createUserHandler));
        _updateUserHandler = updateUserHandler ?? throw new ArgumentNullException(nameof(updateUserHandler));
        _deleteCurrentUserHandler = deleteCurrentUserHandler ?? throw new ArgumentNullException(nameof(deleteCurrentUserHandler));
        _getUsersHandler = getUsersHandler ?? throw new ArgumentNullException(nameof(getUsersHandler));
        _getCurrentUserHandler = getCurrentUserHandler ?? throw new ArgumentNullException(nameof(getCurrentUserHandler));
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

    [HttpPost]
    public async Task RegisterUser(CreateUserCommand command)
    {
        await _createUserHandler.Execute(command);
    }
}
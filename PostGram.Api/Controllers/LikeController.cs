using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostGram.Common.Constants;
using PostGram.Common.Interfaces.Base.Commands;
using PostGram.Common.Requests.Commands;

namespace PostGram.Api.Controllers;

[ApiExplorerSettings(GroupName = EndpointApiNames.Api)]
[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
public class LikeController : ControllerBase
{
    private readonly ICommandHandler<CreateLikeCommand> _createLikeHandler;
    private readonly ICommandHandler<UpdateLikeCommand> _updateLikeHandler;

    public LikeController(ICommandHandler<CreateLikeCommand> createLikeHandler,
        ICommandHandler<UpdateLikeCommand> updateLikeHandler)
    {
        _createLikeHandler = createLikeHandler ;
        _updateLikeHandler = updateLikeHandler;
    }

    [HttpPost]
    public async Task CreateLike(CreateLikeCommand command)
    {
        await _createLikeHandler.Execute(command);
    }

    [HttpPut]
    public async Task UpdateLike(UpdateLikeCommand command)
    {
        await _updateLikeHandler.Execute(command);
    }
}
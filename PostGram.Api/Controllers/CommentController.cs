using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostGram.BLL.Features.GetComment;
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
public class CommentController : ControllerBase
{
    // TODO Вопрос. Получается больше 4 зависимостей, что делать если их будет еще больше, пилить еще или делать фасадные сервисы?
    private readonly ICommandHandler<CreateCommentCommand> _createCommentHandler;
    private readonly ICommandHandler<DeleteCommentCommand> _deleteCommentHandler;
    private readonly IQueryHandler<GetCommentQuery, GetCommentResult> _getCommentHandler;
    private readonly IQueryHandler<GetCommentsForPostQuery, GetCommentsForPostResult> _getCommentsForPostHandler;
    private readonly ICommandHandler<UpdateCommentCommand> _updateCommentHandler;

    public CommentController(
        ICommandHandler<CreateCommentCommand> createCommentHandler,
        ICommandHandler<DeleteCommentCommand> deleteCommentHandler,
        IQueryHandler<GetCommentQuery, GetCommentResult> getCommentHandler,
        IQueryHandler<GetCommentsForPostQuery, GetCommentsForPostResult> getCommentsForPostHandler,
        ICommandHandler<UpdateCommentCommand> updateCommentHandler)
    {
        _createCommentHandler = createCommentHandler ;
        _deleteCommentHandler = deleteCommentHandler;
        _getCommentHandler = getCommentHandler;
        _getCommentsForPostHandler = getCommentsForPostHandler;
        _updateCommentHandler = updateCommentHandler;
    }

    [HttpPost]
    public async Task CreateComment(CreateCommentCommand command)
    {
        await _createCommentHandler.Execute(command);
    }

    [HttpDelete]
    public async Task DeleteComment(DeleteCommentCommand command)
    {
        await _deleteCommentHandler.Execute(command);
    }

    [HttpGet]
    public async Task<GetCommentResult> GetComment(GetCommentQuery query)
    {
        return await _getCommentHandler.Execute(query);
    }

    [HttpGet]
    public async Task<GetCommentsForPostResult> GetCommentsForPost(GetCommentsForPostQuery query)
    {
        return await _getCommentsForPostHandler.Execute(query);
    }

    [HttpPut]
    public async Task UpdateComment(UpdateCommentCommand command)
    {
        await _updateCommentHandler.Execute(command);
    }
}
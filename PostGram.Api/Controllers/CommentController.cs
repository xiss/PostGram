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
        _createCommentHandler = createCommentHandler ?? throw new ArgumentNullException(nameof(createCommentHandler));
        _deleteCommentHandler = deleteCommentHandler ?? throw new ArgumentNullException(nameof(deleteCommentHandler));
        _getCommentHandler = getCommentHandler ?? throw new ArgumentNullException(nameof(getCommentHandler));
        _getCommentsForPostHandler = getCommentsForPostHandler ?? throw new ArgumentNullException(nameof(getCommentsForPostHandler));
        _updateCommentHandler = updateCommentHandler ?? throw new ArgumentNullException(nameof(updateCommentHandler));
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
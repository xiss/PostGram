using PostGram.BLL.Interfaces.Providers;
using PostGram.BLL.Interfaces.Services;
using PostGram.Common.Exceptions;
using PostGram.Common.Interfaces.Base.Commands;
using PostGram.Common.Requests.Commands;
using PostGram.DAL.Entities;

namespace PostGram.BLL.Handlers.Commands;

public class UpdateCommentHandler : ICommandHandler<UpdateCommentCommand>
{
    private readonly IClaimsProvider _claimsProvider;
    private readonly ICommentService _commentService;

    public UpdateCommentHandler(IClaimsProvider claimsProvider, ICommentService commentService)
    {
        _claimsProvider = claimsProvider ;
        _commentService = commentService;
    }

    public async Task Execute(UpdateCommentCommand command)
    {
        Guid userId = _claimsProvider.GetCurrentUserId();
        Comment comment = await _commentService.GetCommentById(command.Id);
        if (comment.AuthorId != userId)
            throw new AuthorizationPostGramException("Cannot modify comment created by another user");
        comment.Body = command.NewBody;
        comment.Edited = DateTimeOffset.UtcNow;

        await _commentService.UpdateComment(comment);
    }
}
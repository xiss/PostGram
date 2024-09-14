using PostGram.BLL.Interfaces.Base.Commands;
using PostGram.BLL.Interfaces.Providers;
using PostGram.BLL.Interfaces.Services;
using PostGram.Common.Exceptions;
using PostGram.Common.Requests.Commands;
using PostGram.DAL.Entities;

namespace PostGram.BLL.Handlers.Commands;

public class DeleteCommentHandler : ICommandHandler<DeleteCommentCommand>
{
    private readonly IClaimsProvider _claimsProvider;
    private readonly ICommentService _commentService;

    public DeleteCommentHandler(IClaimsProvider claimsProvider, ICommentService commentService)
    {
        _claimsProvider = claimsProvider ?? throw new ArgumentNullException(nameof(claimsProvider));
        _commentService = commentService ?? throw new ArgumentNullException(nameof(commentService));
    }

    public async Task Execute(DeleteCommentCommand command)
    {
        Guid userId = _claimsProvider.GetCurrentUserId();
        Comment comment = await _commentService.GetCommentById(command.CommentId);
        if (comment.AuthorId != userId)
            throw new AuthorizationPostGramException("Cannot delete comment created by another user");
        comment.IsDeleted = true;
        await _commentService.UpdateComment(comment);
    }
}
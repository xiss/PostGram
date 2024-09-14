using PostGram.BLL.Interfaces.Base.Commands;

namespace PostGram.Common.Requests.Commands;

public record DeleteCommentCommand : ICommand
{
    public Guid CommentId { get; init; }
}
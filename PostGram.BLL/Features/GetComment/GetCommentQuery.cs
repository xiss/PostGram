using PostGram.Common.Interfaces.Base.Queries;

namespace PostGram.BLL.Features.GetComment;

public record GetCommentQuery : IQuery<GetCommentResult>
{
    public Guid CommentId { get; init; }
}
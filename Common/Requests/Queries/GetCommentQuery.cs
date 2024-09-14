using PostGram.BLL.Interfaces.Base.Queries;

namespace PostGram.Common.Requests.Queries;

public record GetCommentQuery : IQuery
{
    public Guid CommentId { get; init; }
}
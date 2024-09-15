using PostGram.BLL.Interfaces.Base.Queries;
using PostGram.Common.Dtos.User;

namespace PostGram.Common.Requests.Queries;

public record GetCommentQuery : IQuery
{
    public Guid CommentId { get; init; }
}
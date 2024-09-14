using PostGram.BLL.Interfaces.Base.Queries;
using PostGram.Common.Dtos.Comment;

namespace PostGram.Common.Results;

public record GetCommentResult : IQueryResult
{
    public required CommentDto Comment { get; init; }
}
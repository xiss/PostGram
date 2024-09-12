using PostGram.Common.Dtos.Comment;
using PostGram.Common.Interfaces.Base.Queries;

namespace PostGram.Common.Results;

public record GetCommentResult : IQueryResult
{
    public required CommentDto Comment { get; init; }
}
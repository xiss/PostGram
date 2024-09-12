using PostGram.Common.Dtos.Comment;
using PostGram.Common.Interfaces.Base.Queries;

namespace PostGram.Common.Results;

public record GetCommentsForPostResult : IQueryResult
{
    public required List<CommentDto> Comments { get; init; }
}
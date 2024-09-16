using PostGram.Common.Dtos;
using PostGram.Common.Interfaces.Base.Queries;

namespace PostGram.Common.Results;

public record GetCommentsForPostResult : IQueryResult
{
    public required List<CommentDto> Comments { get; init; }
}
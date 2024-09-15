using PostGram.BLL.Interfaces.Base.Queries;
using PostGram.Common.Dtos;

namespace PostGram.Common.Results;

public record GetCommentsForPostResult : IQueryResult
{
    public required List<CommentDto> Comments { get; init; }
}
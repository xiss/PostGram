using PostGram.BLL.Interfaces.Base.Queries;
using PostGram.Common.Dtos;

namespace PostGram.Common.Results;

public record GetCommentResult : IQueryResult
{
    public required CommentDto Comment { get; init; }
}
using PostGram.Common.Dtos;
using PostGram.Common.Interfaces.Base.Queries;

namespace PostGram.BLL.Features.GetComment;

public record GetCommentResult : IQueryResult
{
    public required CommentDto Comment { get; init; }
}
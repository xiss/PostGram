using PostGram.BLL.Interfaces.Base.Queries;
using PostGram.Common.Dtos.Post;

namespace PostGram.Common.Results;

public record GetPostResult : IQueryResult
{
    public required PostDto Post { get; init; }
}
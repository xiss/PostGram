using PostGram.BLL.Interfaces.Base.Queries;
using PostGram.Common.Dtos.Post;

namespace PostGram.Common.Results;

public record GetPostsResult : IQueryResult
{
    public required List<PostDto> Posts { get; init; }
}
using PostGram.Common.Interfaces.Base.Queries;
using PostGram.Common.Results;

namespace PostGram.Common.Requests.Queries;

public record GetPostsQuery : IQuery<GetPostsResult>
{
    public int TakeAmount { get; init; }
    public int SkipAmount { get; init; }
}
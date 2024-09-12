using PostGram.Common.Interfaces.Base.Queries;

namespace PostGram.Common.Requests.Queries;

public record GetPostsQuery : IQuery
{
    public int TakeAmount { get; init; }
    public int SkipAmount { get; init; }
}
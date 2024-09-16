using PostGram.Common.Interfaces.Base.Queries;
using PostGram.Common.Results;

namespace PostGram.Common.Requests.Queries;

public record GetPostQuery : IQuery<GetPostResult>
{
    public Guid PostId { get; init; }
}
using PostGram.BLL.Interfaces.Base.Queries;

namespace PostGram.Common.Requests.Queries;

public record GetPostQuery : IQuery
{
    public Guid PostId { get; init; }
}
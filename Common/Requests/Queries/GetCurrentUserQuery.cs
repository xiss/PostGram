using PostGram.Common.Interfaces.Base.Queries;
using PostGram.Common.Results;

namespace PostGram.Common.Requests.Queries;

public record GetCurrentUserQuery : IQuery<GetCurrentUserResult>
{
}
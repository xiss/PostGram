using PostGram.Common.Dtos;
using PostGram.Common.Interfaces.Base.Queries;

namespace PostGram.Common.Results;

public record GetCurrentUserResult : IQueryResult
{
    public UserDto User { get; init; }
}
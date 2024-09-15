using PostGram.BLL.Interfaces.Base.Queries;
using PostGram.Common.Dtos;

namespace PostGram.Common.Results;

public record GetCurrentUserResult : IQueryResult
{
    public UserDto User { get; init; }
}
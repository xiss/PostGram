using PostGram.Common.Dtos;
using PostGram.Common.Interfaces.Base.Queries;

namespace PostGram.Common.Results;

public record GetUsersResult : IQueryResult
{
    public List<UserDto> Users { get; init; }
}
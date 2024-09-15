using PostGram.BLL.Interfaces.Base.Queries;
using PostGram.Common.Dtos;

namespace PostGram.Common.Results;

public record GetUsersResult : IQueryResult
{
    public List<UserDto> Users { get; init; }
}
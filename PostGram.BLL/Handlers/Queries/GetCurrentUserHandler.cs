using AutoMapper;
using PostGram.BLL.Interfaces.Providers;
using PostGram.BLL.Interfaces.Services;
using PostGram.Common.Dtos;
using PostGram.Common.Interfaces.Base.Queries;
using PostGram.Common.Requests.Queries;
using PostGram.Common.Results;
using PostGram.DAL.Entities;

namespace PostGram.BLL.Handlers.Queries;

public class GetCurrentUserHandler : IQueryHandler<GetCurrentUserQuery, GetCurrentUserResult>
{
    private readonly IMapper _mapper;
    private readonly IClaimsProvider _claimsProvider;
    private readonly IUserService _userService;

    public GetCurrentUserHandler(IMapper mapper, IClaimsProvider claimsProvider, IUserService userService)
    {
        _mapper = mapper ;
        _claimsProvider = claimsProvider;
        _userService = userService;
    }

    public async Task<GetCurrentUserResult> Execute(GetCurrentUserQuery query)
    {
        Guid userId = _claimsProvider.GetCurrentUserId();
        User user = await _userService.GetUserById(userId);
        return new GetCurrentUserResult() { User = _mapper.Map<UserDto>(user) };
    }
}
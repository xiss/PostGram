using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using PostGram.Common.Dtos;
using PostGram.Common.Exceptions;
using PostGram.Common.Interfaces.Base.Queries;
using PostGram.Common.Requests.Queries;
using PostGram.Common.Results;
using PostGram.DAL;

namespace PostGram.BLL.Handlers.Queries;

public class GetUsersHandler : IQueryHandler<GetUsersQuery, GetUsersResult>
{
    private readonly IMapper _mapper;
    private readonly DataContext _dataContext;


    public GetUsersHandler(IMapper mapper, DataContext dataContext)
    {
        _mapper = mapper ;
        _dataContext = dataContext;
    }

    public async Task<GetUsersResult> Execute(GetUsersQuery query)
    {
        List<UserDto> models = await _dataContext
            .Users
            .Include(x => x.Avatar)
            .AsNoTracking()
            .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        if (models.Count == 0)
            throw new NotFoundPostGramException("Users not found in DB");
        return new GetUsersResult() { Users = models };
    }
}
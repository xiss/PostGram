using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using PostGram.BLL.Interfaces.Base.Queries;
using PostGram.BLL.Interfaces.Providers;
using PostGram.Common.Dtos.Subscription;
using PostGram.Common.Exceptions;
using PostGram.Common.Requests.Queries;
using PostGram.Common.Results;
using PostGram.DAL;

namespace PostGram.BLL.Handlers.Queries;

public class GetMasterSubscriptionsHandler : IQueryHandler<GetMasterSubscriptionsQuery, GetMasterSubscriptionsResult>
{
    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;
    private readonly IClaimsProvider _claimsProvider;

    public GetMasterSubscriptionsHandler(DataContext dataContext, IMapper mapper, IClaimsProvider claimsProvider)
    {
        _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _claimsProvider = claimsProvider ?? throw new ArgumentNullException(nameof(claimsProvider));
    }

    public async Task<GetMasterSubscriptionsResult> Execute(GetMasterSubscriptionsQuery query)
    {
        Guid userId = _claimsProvider.GetCurrentUserId();
        List<SubscriptionDto> subscriptions = await _dataContext.Subscriptions
            .Where(s => s.MasterId == userId)
            .ProjectTo<SubscriptionDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
        if (subscriptions.Count == 0)
            throw new NotFoundPostGramException($"Not found master subscription for user {userId}");

        return new GetMasterSubscriptionsResult() { SubscriptionDtos = subscriptions };
    }
}
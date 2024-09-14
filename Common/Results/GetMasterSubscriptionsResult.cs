using PostGram.BLL.Interfaces.Base.Queries;
using PostGram.Common.Dtos.Subscription;

namespace PostGram.Common.Results;

public record GetMasterSubscriptionsResult : IQueryResult
{
    public required List<SubscriptionDto> SubscriptionDtos { get; init; }
}
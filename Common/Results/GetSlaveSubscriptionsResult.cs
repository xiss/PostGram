using PostGram.Common.Dtos;
using PostGram.Common.Interfaces.Base.Queries;

namespace PostGram.Common.Results;

public record GetSlaveSubscriptionsResult : IQueryResult
{
    public required List<SubscriptionDto> SubscriptionDtos { get; init; }
}
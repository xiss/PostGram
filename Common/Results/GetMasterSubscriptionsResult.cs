using PostGram.BLL.Interfaces.Base.Queries;
using PostGram.Common.Dtos;

namespace PostGram.Common.Results;

public record GetMasterSubscriptionsResult : IQueryResult
{
    public required List<SubscriptionDto> SubscriptionDtos { get; init; }
}
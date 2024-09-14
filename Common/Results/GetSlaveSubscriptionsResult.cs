﻿using PostGram.BLL.Interfaces.Base.Queries;
using PostGram.Common.Dtos.Subscription;

namespace PostGram.Common.Results;

public record GetSlaveSubscriptionsResult : IQueryResult
{
    public required List<SubscriptionDto> SubscriptionDtos { get; init; }
}
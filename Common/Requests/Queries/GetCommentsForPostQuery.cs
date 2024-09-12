﻿using PostGram.Common.Interfaces.Base.Queries;

namespace PostGram.Common.Requests.Queries;

public record GetCommentsForPostQuery : IQuery
{
    public Guid PostId { get; init; }
}
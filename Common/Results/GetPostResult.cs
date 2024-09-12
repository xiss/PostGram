﻿using PostGram.Common.Dtos.Post;
using PostGram.Common.Interfaces.Base.Queries;

namespace PostGram.Common.Results;

public record GetPostResult : IQueryResult
{
    public required PostDto Post { get; init; }
}
﻿using PostGram.Common.Dtos.Post;
using PostGram.Common.Interfaces.Base.Queries;

namespace PostGram.Common.Results;

public record GetPostsResult : IQueryResult
{
    public required List<PostDto> Posts { get; init; }
}
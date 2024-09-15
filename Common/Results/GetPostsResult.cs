﻿using PostGram.BLL.Interfaces.Base.Queries;
using PostGram.Common.Dtos;

namespace PostGram.Common.Results;

public record GetPostsResult : IQueryResult
{
    public required List<PostDto> Posts { get; init; }
}
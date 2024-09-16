﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PostGram.BLL.Interfaces.Providers;
using PostGram.BLL.Interfaces.Services;
using PostGram.Common.Dtos;
using PostGram.Common.Exceptions;
using PostGram.Common.Interfaces.Base.Queries;
using PostGram.Common.Requests.Queries;
using PostGram.Common.Results;
using PostGram.DAL;
using PostGram.DAL.Entities;

namespace PostGram.BLL.Handlers.Queries;

public class GetPostsHandler : IQueryHandler<GetPostsQuery, GetPostsResult>
{
    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;
    private readonly IClaimsProvider _claimsProvider;
    private readonly ISubscriptionsService _subscriptionsService;

    public GetPostsHandler(DataContext dataContext, IMapper mapper, IClaimsProvider claimsProvider,
        ISubscriptionsService subscriptionsService)
    {
        _dataContext = dataContext;
        _mapper = mapper;
        _claimsProvider = claimsProvider;
        _subscriptionsService = subscriptionsService;
    }

    public async Task<GetPostsResult> Execute(GetPostsQuery query)
    {
        Guid userId = _claimsProvider.GetCurrentUserId();
        List<Guid> subscriptions = await _subscriptionsService.GetAvailableSubscriptionsForSlaveUser(userId);

        List<Post> posts = await _dataContext.Posts
            .Include(p => p.PostContents)
            .Include(p => p.Author)
            .ThenInclude(u => u.Slaves)
            .Include(p => p.Comments)
            .ThenInclude(c => c.Likes)
            .Include(p => p.Likes)
            .Include(p => p.Author)
            .ThenInclude(a => a.Avatar)
            .Where(p => subscriptions
                    .Contains(p.AuthorId)
                || p.AuthorId == userId)
            .OrderByDescending(p => p.Created)
            .Skip(query.SkipAmount)
            .Take(query.TakeAmount)
            .AsNoTracking()
            .ToListAsync();

        if (posts.Count == 0)
            throw new NotFoundPostGramException("Posts not found");

        var result = posts.Select(p => _mapper.Map<PostDto>(p)).ToList();
        foreach (PostDto post in result)
        {
            post.LikeByUser = _mapper
                .Map<LikeDto>(posts
                    .FirstOrDefault(p => p.Id == post.Id)
                    ?.Likes
                    .FirstOrDefault(l => l.AuthorId == userId));
        }

        return new GetPostsResult { Posts = result };
    }
}
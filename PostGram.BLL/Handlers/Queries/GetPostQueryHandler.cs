using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PostGram.BLL.Interfaces.Base.Queries;
using PostGram.BLL.Interfaces.Providers;
using PostGram.BLL.Interfaces.Services;
using PostGram.Common.Dtos.Like;
using PostGram.Common.Dtos.Post;
using PostGram.Common.Exceptions;
using PostGram.Common.Requests.Queries;
using PostGram.Common.Results;
using PostGram.DAL;
using PostGram.DAL.Entities;

namespace PostGram.BLL.Handlers.Queries;

public class GetPostQueryHandler : IQueryHandler<GetPostQuery, GetPostResult>
{
    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;
    private readonly IClaimsProvider _claimsProvider;
    private readonly ISubscriptionsService _subscriptionsService;

    public GetPostQueryHandler(DataContext dataContext, IMapper mapper, IClaimsProvider claimsProvider,
        ISubscriptionsService subscriptionsService)
    {
        _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _claimsProvider = claimsProvider ?? throw new ArgumentNullException(nameof(claimsProvider));
        _subscriptionsService = subscriptionsService ?? throw new ArgumentNullException(nameof(subscriptionsService));
    }

    public async Task<GetPostResult> Execute(GetPostQuery query)
    {
        Guid userId = _claimsProvider.GetCurrentUserId();
        List<Guid> subscriptions = await _subscriptionsService.GetAvailableSubscriptionsForSlaveUser(userId);

        Post post = await _dataContext.Posts
                .AsNoTracking()
                .Include(p => p.PostContents)
                .Include(p => p.Likes)
                .Include(p => p.Author)
                .ThenInclude(a => a.Avatar)
                .Include(p => p.Comments
                    .OrderBy(c => c.Created))
                .ThenInclude(c => c.Likes)
                .FirstOrDefaultAsync(p => p.Id == query.PostId)
            ?? throw new NotFoundPostGramException("Post not found: " + query.PostId);
        if (post.AuthorId != userId && !subscriptions.Contains(post.AuthorId))
            throw new AuthorizationPostGramException($"User {userId} cannot access post {post.Id}");

        PostDto result = _mapper.Map<PostDto>(post);

        result.LikeByUser = _mapper
            .Map<LikeDto>(post.Likes
                .FirstOrDefault(l => l.AuthorId == userId));
        return new GetPostResult { Post = result };
    }
}
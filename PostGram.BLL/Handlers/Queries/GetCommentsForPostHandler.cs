using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PostGram.BLL.Interfaces.Base.Queries;
using PostGram.BLL.Interfaces.Providers;
using PostGram.Common.Dtos;
using PostGram.Common.Exceptions;
using PostGram.Common.Requests.Queries;
using PostGram.Common.Results;
using PostGram.DAL;
using PostGram.DAL.Entities;

namespace PostGram.BLL.Handlers.Queries;

public class GetCommentsForPostHandler : IQueryHandler<GetCommentsForPostQuery, GetCommentsForPostResult>
{
    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;
    private readonly IClaimsProvider _claimsProvider;

    public GetCommentsForPostHandler(DataContext dataContext, IMapper mapper, IClaimsProvider claimsProvider)
    {
        _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _claimsProvider = claimsProvider ?? throw new ArgumentNullException(nameof(claimsProvider));
    }

    public async Task<GetCommentsForPostResult> Execute(GetCommentsForPostQuery query)
    {
        Guid userId = _claimsProvider.GetCurrentUserId();
        Comment[] comments = await _dataContext.Comments
            .Include(c => c.Likes)
            .Include(c => c.Author)
            .ThenInclude(a => a.Avatar)
            .AsNoTracking()
            .Where(c => c.PostId == query.PostId)
            .OrderBy(c => c.Created)
            .ToArrayAsync();
        if (comments.Length == 0)
            throw new NotFoundPostGramException("Comments not found for post: " + query.PostId);

        List<CommentDto>? result = _mapper.Map<List<CommentDto>>(comments);
        foreach (CommentDto comment in result)
        {
            comment.LikeByUser = _mapper
                .Map<LikeDto>(comments
                    .FirstOrDefault(c => c.Id == comment.Id)
                    ?.Likes
                    .FirstOrDefault(l => l.AuthorId == userId));
        }

        return new GetCommentsForPostResult { Comments = result };
    }
}
using AutoMapper;
using PostGram.BLL.Interfaces.Base.Queries;
using PostGram.BLL.Interfaces.Providers;
using PostGram.BLL.Interfaces.Services;
using PostGram.Common.Dtos.Comment;
using PostGram.Common.Dtos.Like;
using PostGram.Common.Requests.Queries;
using PostGram.Common.Results;
using PostGram.DAL.Entities;

namespace PostGram.BLL.Handlers.Queries;

public class GetCommentQueryHandler : IQueryHandler<GetCommentQuery, GetCommentResult>
{
    private readonly IMapper _mapper;
    private readonly IClaimsProvider _claimsProvider;
    private readonly ICommentService _commentService;

    public GetCommentQueryHandler(IMapper mapper, IClaimsProvider claimsProvider, ICommentService commentService)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _claimsProvider = claimsProvider ?? throw new ArgumentNullException(nameof(claimsProvider));
        _commentService = commentService ?? throw new ArgumentNullException(nameof(commentService));
    }

    public async Task<GetCommentResult> Execute(GetCommentQuery query)
    {
        Guid userId = _claimsProvider.GetCurrentUserId();
        Comment comment = await _commentService.GetCommentById(query.CommentId);
        CommentDto result = _mapper.Map<CommentDto>(comment);

        result.LikeByUser = _mapper
            .Map<LikeDto>(comment.Likes
                .FirstOrDefault(l => l.AuthorId == userId));
        return new GetCommentResult { Comment = result };
    }
}
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PostGram.BLL.Interfaces.Base.Commands;
using PostGram.BLL.Interfaces.Providers;
using PostGram.Common.Enums;
using PostGram.Common.Exceptions;
using PostGram.Common.Requests.Commands;
using PostGram.DAL;
using PostGram.DAL.Entities;

namespace PostGram.BLL.Handlers.Commands;

public class CreateLikeHandler : ICommandHandler<CreateLikeCommand>
{
    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;
    private readonly IClaimsProvider _claimsProvider;

    public CreateLikeHandler(DataContext dataContext, IMapper mapper, IClaimsProvider claimsProvider)
    {
        _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _claimsProvider = claimsProvider ?? throw new ArgumentNullException(nameof(claimsProvider));
    }

    public async Task Execute(CreateLikeCommand command)
    {
        Like like = _mapper.Map<Like>(command);
        Guid userId = _claimsProvider.GetCurrentUserId();
        like.AuthorId = userId;

        if (await CheckLikeExist(userId, like.EntityId))
            throw new UnprocessableRequestPostGramException(
                $"User {like.AuthorId} already has like for entity '{like.EntityType}' - {like.EntityId}");

        switch (like.EntityType)
        {
            case LikableEntities.Post:
                Post post = await _dataContext.Posts.FirstOrDefaultAsync(p => p.Id == like.EntityId)
                    ?? throw new NotFoundPostGramException($"Post {like.EntityId} not found");

                post.Likes.Add(like);
                break;

            case LikableEntities.Comment:
                Comment comment = await _dataContext.Comments.FirstOrDefaultAsync(c => c.Id == like.EntityId)
                    ?? throw new NotFoundPostGramException($"Comment {like.EntityId} not found");

                comment.Likes.Add(like);
                break;

            default:
                throw new PostGramException("Unregistered entity type");
        }

        _dataContext.Likes.Add(like);
        await _dataContext.SaveChangesAsync();
    }

    private async Task<bool> CheckLikeExist(Guid authorId, Guid entityId)
    {
        return await _dataContext.Likes.AnyAsync(l => l.AuthorId == authorId && l.EntityId == entityId);
    }
}
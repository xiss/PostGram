using Microsoft.EntityFrameworkCore;
using PostGram.BLL.Interfaces.Base.Commands;
using PostGram.BLL.Interfaces.Providers;
using PostGram.BLL.Interfaces.Services;
using PostGram.Common.Exceptions;
using PostGram.Common.Requests.Commands;
using PostGram.DAL;
using PostGram.DAL.Entities;

namespace PostGram.BLL.Handlers.Commands;

public class DeletePostHandler : ICommandHandler<DeletePostCommand>
{
    private readonly DataContext _dataContext;
    private readonly IClaimsProvider _claimsProvider;
    private readonly IPostService _postService;

    public DeletePostHandler(DataContext dataContext, IClaimsProvider claimsProvider, IPostService postService)
    {
        _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        _claimsProvider = claimsProvider ?? throw new ArgumentNullException(nameof(claimsProvider));
        _postService = postService ?? throw new ArgumentNullException(nameof(postService));
    }

    public async Task Execute(DeletePostCommand command)
    {
        Guid userId = _claimsProvider.GetCurrentUserId();
        Post post = await _dataContext.Posts
                .Include(p => p.PostContents)
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.Id == command.PostId)
            ?? throw new NotFoundPostGramException($"Post {command.PostId} not found");

        if (post.AuthorId != userId)
            throw new AuthorizationPostGramException("Cannot delete post created by another user id");

        post.IsDeleted = true;
        await _postService.DeletePostContents(post.PostContents);

        _dataContext.Posts.Update(post);
        await _dataContext.SaveChangesAsync();
    }
}
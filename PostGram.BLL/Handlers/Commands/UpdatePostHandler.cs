using Microsoft.EntityFrameworkCore;
using PostGram.BLL.Interfaces.Base.Commands;
using PostGram.BLL.Interfaces.Providers;
using PostGram.BLL.Interfaces.Services;
using PostGram.Common.Dtos.Attachment;
using PostGram.Common.Exceptions;
using PostGram.Common.Requests.Commands;
using PostGram.DAL;
using PostGram.DAL.Entities;

namespace PostGram.BLL.Handlers.Commands;

public class UpdatePostHandler : ICommandHandler<UpdatePostCommand>
{
    private readonly DataContext _dataContext;
    private readonly IClaimsProvider _claimsProvider;
    private readonly IPostService _postService;
    private readonly IAttachmentService _attachmentService;

    public UpdatePostHandler(DataContext dataContext, IClaimsProvider claimsProvider, IPostService postService,
        IAttachmentService attachmentService)
    {
        _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        _claimsProvider = claimsProvider ?? throw new ArgumentNullException(nameof(claimsProvider));
        _postService = postService ?? throw new ArgumentNullException(nameof(postService));
        _attachmentService = attachmentService;
    }

    public async Task Execute(UpdatePostCommand command)
    {
        Guid userId = _claimsProvider.GetCurrentUserId();
        Post post = await _dataContext.Posts
                .Include(p => p.PostContents)
                .Include(p => p.Likes)
                .Include(p => p.Author)
                .ThenInclude(a => a.Avatar)
                .Include(p => p.Comments
                    .OrderBy(c => c.Created))
                .FirstOrDefaultAsync(u => u.Id == command.Id)
            ?? throw new NotFoundPostGramException("Post not found: " + command.Id);

        if (post.AuthorId != userId)
            throw new AuthorizationPostGramException("Cannot modify post created by another user");

        //Body
        if (command.UpdatedBody != null)
            post.Body = command.UpdatedBody;

        //Header
        if (command.UpdatedHeader != null)
            post.Header = command.UpdatedHeader;

        //Add new content
        if (command.NewContent.Count > 0)
            foreach (MetadataModel postContent in command.NewContent)
            {
                //Если это повторный запрос и мы уже добавили, второй раз не добавляем.
                if (post.PostContents.Any(pc => pc.Id == postContent.TempId))
                    continue;

                _dataContext.PostContents.Add(new PostContent
                {
                    Id = postContent.TempId,
                    PostId = post.Id,
                    AuthorId = userId,
                    Created = DateTimeOffset.UtcNow,
                    MimeType = postContent.MimeType,
                    Name = postContent.Name,
                    Size = postContent.Size
                });
                await _attachmentService.ApplyFile(postContent.TempId.ToString());
            }

        //Edit timestamp
        post.Edited = DateTimeOffset.UtcNow;

        //Delete postContent
        if (command.ContentToDelete.Count > 0)
        {
            List<PostContent> postContentsToDelete =
                await GetPostContentsByIds(command.ContentToDelete.Select(am => am.Id).ToList());
            await _postService.DeletePostContents(postContentsToDelete);
        }

        await _dataContext.SaveChangesAsync();
    }

    private async Task<List<PostContent>> GetPostContentsByIds(List<Guid> postContentIds)
    {
        if (postContentIds.Count == 0 || postContentIds == null)
            throw new ArgumentPostGramException("Input collection is empty or null");

        List<PostContent> postContent = await _dataContext.PostContents
            .Where(pc => postContentIds.Contains(pc.Id))
            .ToListAsync();

        return postContent
            ?? throw new NotFoundPostGramException($"Post contents {string.Join(' ', postContentIds)} not found in DB");
    }
}
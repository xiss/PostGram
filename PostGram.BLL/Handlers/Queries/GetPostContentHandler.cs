using Microsoft.EntityFrameworkCore;
using PostGram.BLL.Interfaces.Providers;
using PostGram.BLL.Interfaces.Services;
using PostGram.Common.Configs;
using PostGram.Common.Dtos.Attachment;
using PostGram.Common.Exceptions;
using PostGram.Common.Interfaces.Base.Queries;
using PostGram.Common.Requests.Queries;
using PostGram.DAL;
using PostGram.DAL.Entities;

namespace PostGram.BLL.Handlers.Queries;

public class GetPostContentHandler : IQueryHandler<GetPostContentQuery, FileInfoDto>
{
    private readonly DataContext _dataContext;
    private readonly IAttachmentService _attachmentService;
    private readonly AppConfig _appConfig;
    private readonly IClaimsProvider _claimsProvider;


    public GetPostContentHandler(IAttachmentService attachmentService,
        AppConfig appConfig,
        DataContext dataContext,
        IClaimsProvider claimsProvider)
    {
        _attachmentService = attachmentService;
        _appConfig = appConfig;
        _dataContext = dataContext;
        _claimsProvider = claimsProvider;
    }

    public async Task<FileInfoDto> Execute(GetPostContentQuery query)
    {
        Guid userId = _claimsProvider.GetCurrentUserId();

        PostContent? postContent = await _dataContext.PostContents
            .Include(pc => pc.Author)
            .ThenInclude(u => u.Masters)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == query.PostContentId);
        if (postContent == null)
            throw new NotFoundPostGramException("PostContent not found in DB: " + query.PostContentId);

        if (!_attachmentService.CheckAttachmentsExists(postContent.Id.ToString()))
            throw new NotFoundPostGramException("File not found: " + query.PostContentId);

        // TODO Этого тут быть не должно
        if (postContent.Author.IsPrivate && postContent.AuthorId != userId &&
            !postContent.Author.Masters.Any(s => s.SlaveId == userId && s.Status))
            throw new AuthorizationPostGramException("Access denied");

        string filePath = Path.Combine(_appConfig.AttachmentsFolderPath, postContent.Id.ToString());

        return new FileInfoDto()
        {
            MimeType = postContent.MimeType,
            Name = postContent.Name,
            FileStream = new FileStream(filePath, FileMode.Open)
        };
    }
}
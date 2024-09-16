using Microsoft.EntityFrameworkCore;
using PostGram.BLL.Features.GetComment;
using PostGram.BLL.Interfaces.Services;
using PostGram.Common.Configs;
using PostGram.Common.Dtos.Attachment;
using PostGram.Common.Exceptions;
using PostGram.Common.Interfaces.Base.Queries;
using PostGram.Common.Requests.Queries;
using PostGram.DAL;
using PostGram.DAL.Entities;

namespace PostGram.BLL.Handlers.Queries;

public class GetAvatarForUserHandler : IQueryHandler<GetAvatarForUserQuery, FileInfoDto>
{
    private readonly DataContext _dataContext;
    private readonly IAttachmentService _attachmentService;
    private readonly AppConfig _appConfig;

    public GetAvatarForUserHandler(IAttachmentService attachmentService, AppConfig appConfig, DataContext dataContext)
    {
        _attachmentService = attachmentService;
        _appConfig = appConfig;
        _dataContext = dataContext;
    }

    public async Task<FileInfoDto> Execute(GetAvatarForUserQuery query)
    {
        Avatar? avatar = await _dataContext.Avatars.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == query.UserId);
        if (avatar == null)
            throw new NotFoundPostGramException("Avatar not found in DB for user: " + query.UserId);
        if (!_attachmentService.CheckAttachmentsExists(avatar.Id.ToString()))
            throw new NotFoundPostGramException("File not found: " + avatar.Id);

        string filePath = Path.Combine(_appConfig.AttachmentsFolderPath, avatar.Id.ToString());

        return new FileInfoDto()
        {
            MimeType = avatar.MimeType,
            Name = avatar.Name,
            //Path = filePath,
            FileStream = new FileStream(filePath, FileMode.Open)
        };
    }
}
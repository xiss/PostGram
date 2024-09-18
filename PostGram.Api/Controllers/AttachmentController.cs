using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostGram.Common.Dtos.Attachment;
using PostGram.Common.Interfaces.Base.Commands;
using PostGram.Common.Interfaces.Base.Queries;
using PostGram.Common.Requests.Commands;
using PostGram.Common.Requests.Queries;

namespace PostGram.Api.Controllers;

[ApiExplorerSettings(GroupName = Common.Constants.Api.EndpointApiName)]
[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
public class AttachmentController : ControllerBase
{
    private readonly IQueryHandler<GetAvatarForUserQuery, FileInfoDto> _getAvatarForUserHandler;
    private readonly IQueryHandler<GetPostContentQuery, FileInfoDto> _getPostContentHandler;
    private readonly ICommandHandler<UploadAttachmentsCommand> _uploadAttachmentsHandler;

    public AttachmentController(IQueryHandler<GetAvatarForUserQuery, FileInfoDto> getAvatarForUserHandler,
        IQueryHandler<GetPostContentQuery, FileInfoDto> getPostContentHandler,
        ICommandHandler<UploadAttachmentsCommand> uploadAttachmentsHandler)
    {
        _getAvatarForUserHandler = getAvatarForUserHandler;
        _getPostContentHandler = getPostContentHandler;
        _uploadAttachmentsHandler = uploadAttachmentsHandler;
    }

    [HttpGet]
    public async Task<FileStreamResult> GetAvatarForUser(GetAvatarForUserQuery query)
    {
        FileInfoDto fileInfo = await _getAvatarForUserHandler.Execute(query);
        return GetAttachmentStream(fileInfo, query.IsDownload);
    }

    [HttpGet]
    public async Task<FileStreamResult> GetPostContent(GetPostContentQuery query)
    {
        FileInfoDto fileInfo = await _getPostContentHandler.Execute(query);
        return GetAttachmentStream(fileInfo, query.IsDownload);
    }

    [HttpPost]
    public async Task UploadAttachments(UploadAttachmentsCommand command)
    {
        await _uploadAttachmentsHandler.Execute(command);
    }

    private FileStreamResult GetAttachmentStream(FileInfoDto model, bool download)
    {
        return download
            ? File(model.FileStream, model.MimeType, $"{model.Name}.{Path.GetExtension(model.Name)}")
            : File(model.FileStream, model.MimeType);
    }
}
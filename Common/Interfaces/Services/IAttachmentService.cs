using Microsoft.AspNetCore.Http;
using PostGram.Common.Dtos.Attachment;

namespace PostGram.Common.Interfaces.Services;

public interface IAttachmentService
{
    Task<string> ApplyFile(string temporaryFileId);

    void DeleteFile(Guid id);

    Task<FileInfoDto> GetAvatarForUser(Guid userId);

    Task<FileInfoDto> GetPostContent(Guid postContentId, Guid currentUserId);

    Task UploadFile(IFormFile file);
}
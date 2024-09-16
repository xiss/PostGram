using Microsoft.AspNetCore.Http;

namespace PostGram.BLL.Interfaces.Services;

public interface IAttachmentService
{
    Task<string> ApplyFile(string temporaryFileId);
    void DeleteFile(Guid id);
    Task UploadFile(IFormFile file);
    bool CheckAttachmentsExists(string attachmentName);
}
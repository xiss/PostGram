using PostGram.Api.Models;

namespace PostGram.Api.Services
{
    public interface IAttachmentService
    {
        Task<AttachmentModel> GetAvatarForUser(Guid userId);

        Task<string> ApplyFile(string temporaryFileId);

        Task<MetadataModel> UploadFile(IFormFile file);

        Task<AttachmentModel> GetAttachment(Guid attachmentId);
    }
}
using Microsoft.AspNetCore.Http;
using PostGram.Common.Models.Attachment;

namespace PostGram.Common.Interfaces.Services
{
    public interface IAttachmentService
    {
        Task<string> ApplyFile(string temporaryFileId);

        void DeleteFile(Guid id);
        void Dispose();
        Task<FileInfoModel> GetAvatarForUser(Guid userId);

        Task<FileInfoModel> GetPostContent(Guid postContentId, Guid currentUserId);

        Task<MetadataModel> UploadFile(IFormFile file);
    }
}
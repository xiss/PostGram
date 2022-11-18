using PostGram.Api.Models.Attachment;

namespace PostGram.Api.Services
{
    public interface IAttachmentService
    {
        Task<string> ApplyFile(string temporaryFileId);

        void DeleteFile(Guid id);

        Task<FileInfoModel> GetAvatarForUser(Guid userId);

        Task<FileInfoModel> GetPostContent(Guid postContentId);

        Task<MetadataModel> UploadFile(IFormFile file);
    }
}
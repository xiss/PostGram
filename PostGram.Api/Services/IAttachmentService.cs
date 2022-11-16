using PostGram.Api.Models.Attachment;

namespace PostGram.Api.Services
{
    public interface IAttachmentService
    {
        Task<FileInfoModel> GetAvatarForUser(Guid userId);

        Task<string> ApplyFile(string temporaryFileId);

        Task<MetadataModel> UploadFile(IFormFile file);

        Task<FileInfoModel> GetPostContent(Guid postContentId);
        void DeleteFile(Guid id);
    }
}
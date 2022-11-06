using PostGram.Api.Models;

namespace PostGram.Api.Services
{
    public interface IAttachService
    {
        Task<AttachModel> GetAvatarForUser(Guid userId);
        Task<string> MoveToAttaches(string temporaryFileId);
        Task<MetadataModel> UploadFile(IFormFile file);
    }
}
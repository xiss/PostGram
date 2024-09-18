namespace PostGram.BLL.Interfaces.Services;

public interface IAttachmentService
{
    Task<string> ApplyFile(string temporaryFileId);
    void DeleteFile(Guid id);
    bool CheckAttachmentsExists(string attachmentName);
}
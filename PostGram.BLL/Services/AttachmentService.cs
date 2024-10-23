using PostGram.BLL.Interfaces.Services;
using PostGram.Common.Configs;
using PostGram.Common.Exceptions;

namespace PostGram.BLL.Services;

public class AttachmentService : IAttachmentService
{
    private readonly AppConfig _appConfig;

    public AttachmentService(AppConfig appConfig)
    {
        _appConfig = appConfig;
    }

    public async Task<string> ApplyFile(string temporaryFileId)
    {
        FileInfo tempFile = new(Path.Combine(Path.GetTempPath(), temporaryFileId));
        if (!tempFile.Exists)
            throw new NotFoundPostGramException("File not found: " + temporaryFileId);
        DirectoryInfo attachmentsDirectory = GetOrCreateAttachmentsFolder();
        try
        {
            string? destFile = Path.Combine(attachmentsDirectory.FullName, temporaryFileId);
            File.Move(tempFile.FullName, destFile, true);

            return await Task.Run(() => destFile);
        }
        catch (Exception e)
        {
            throw new PostGramException(e.Message, e);
        }
    }

    public void DeleteFile(Guid id)
    {
        FileInfo fileFile = new(Path.Combine(_appConfig.AttachmentsFolderPath, id.ToString()));
        if (!fileFile.Exists)
            throw new NotFoundPostGramException($"File {id} not found");
        try
        {
            fileFile.Delete();
        }
        catch (Exception e)
        {
            throw new PostGramException(e.Message, e);
        }
    }

    public bool CheckAttachmentsExists(string attachmentName)
    {
        return new FileInfo(Path.Combine(_appConfig.AttachmentsFolderPath, attachmentName)).Exists;
    }

    private DirectoryInfo GetOrCreateAttachmentsFolder()
    {
        try
        {
            var directory = new DirectoryInfo(_appConfig.AttachmentsFolderPath);
            if (!directory.Exists)
                directory.Create();
            return directory;
        }
        catch (Exception e)
        {
            throw new PostGramException(e.Message, e);
        }
    }
}
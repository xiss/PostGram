using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PostGram.BLL.Features.GetComment;
using PostGram.BLL.Interfaces.Services;
using PostGram.Common.Configs;
using PostGram.Common.Dtos.Attachment;
using PostGram.Common.Exceptions;
using PostGram.Common.Interfaces.Base.Queries;
using PostGram.DAL;
using PostGram.DAL.Entities;

namespace PostGram.BLL.Services;

public class AttachmentService : IAttachmentService
{
    private readonly AppConfig _appConfig;
    private readonly DataContext _dataContext;

    public AttachmentService(DataContext dataContext, IOptions<AppConfig> appConfig)
    {
        _dataContext = dataContext;
        _appConfig = appConfig.Value;
    }

    public async Task<string> ApplyFile(string temporaryFileId)
    {
        FileInfo tempFile = new(Path.Combine(Path.GetTempPath(), temporaryFileId));
        if (!tempFile.Exists)
            throw new NotFoundPostGramException("File not found: " + temporaryFileId);
        DirectoryInfo attachmentsDirectory = GetOrCreateAttachmentsFolder();
        try
        {
            var destFile = Path.Combine(attachmentsDirectory.FullName, temporaryFileId);
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

   

    

    // TODO попробовать использовать S3
    public async Task UploadFile(IFormFile file)
    {
        MetadataDto model = new()
        {
            TempId = Guid.NewGuid(),
            Name = file.FileName,
            MimeType = file. ContentType,
            Size = file.Length
        };
        string newPath = Path.Combine(Path.GetTempPath(), model.TempId.ToString());

        using (var stream = File.Create(newPath))
        {
            await file.CopyToAsync(stream);
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
            DirectoryInfo directory = new DirectoryInfo(_appConfig.AttachmentsFolderPath);
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
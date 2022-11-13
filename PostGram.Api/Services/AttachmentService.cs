using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PostGram.Api.Configs;
using PostGram.Api.Models.Attachment;
using PostGram.Common.Exceptions;
using PostGram.DAL;
using PostGram.DAL.Entities;

namespace PostGram.Api.Services
{
    public class AttachmentService : IDisposable, IAttachmentService
    {
        private readonly DataContext _dataContext;
        private readonly AppConfig _appConfig;

        public AttachmentService(DataContext dataContext, IOptions<AppConfig> appConfig)
        {
            _dataContext = dataContext;
            _appConfig = appConfig.Value;
        }

        public void Dispose()
        {
            _dataContext.Dispose();
        }

        public async Task<MetadataModel> UploadFile(IFormFile file)
        {
            MetadataModel model = new()
            {
                TempId = Guid.NewGuid(),
                Name = file.FileName,
                MimeType = file.ContentType, //TODO 3 Сделать распознование по первой  строке файла
                Size = file.Length
            };
            string newPath = Path.Combine(Path.GetTempPath(), model.TempId.ToString());
            FileInfo fileInfo = new FileInfo(newPath);

            using (var stream = System.IO.File.Create(newPath))
            {
                await file.CopyToAsync(stream);
            }

            return model;
        }

        public async Task<string> ApplyFile(string temporaryFileId)
        {
            FileInfo tempFile = new(Path.Combine(Path.GetTempPath(), temporaryFileId));
            if (!tempFile.Exists)
                throw new NotFoundPostGramException("File not found: " + tempFile.FullName);
            try
            {
                var destFile = Path.Combine(Directory.GetCurrentDirectory(), _appConfig.AttachesFolderName,
                    temporaryFileId);
                var destFi = new FileInfo(destFile);
                if (destFi.Directory != null && !destFi.Directory.Exists)
                    destFi.Directory.Create();

                File.Move(tempFile.FullName, destFile, true);

                return await Task.Run(() => destFile);
            }
            catch (Exception e)
            {
                throw new FilePostGramException(e.Message, e);
            }
        }

        public async Task<FileInfoModel> GetAvatarForUser(Guid userId)
        {
            Avatar? avatar = await _dataContext.Avatars.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId);
            if (avatar == null)
                throw new NotFoundPostGramException("Avatar not found in DB for user: " + userId);

            if (!FileExists(avatar.FilePath))
                throw new NotFoundPostGramException("File not found: " + avatar.FilePath);

            return new FileInfoModel(avatar.Name, avatar.MimeType, avatar.FilePath);
        }

        public async Task<FileInfoModel> GetPostContent(Guid postContentId)
        {
            PostContent? postContent = await _dataContext.PostContents.AsNoTracking().FirstOrDefaultAsync(a => a.Id == postContentId);
            if (postContent == null)
                throw new NotFoundPostGramException("PostContent not found in DB: " + postContentId);

            if (!FileExists(postContent.FilePath))
                throw new NotFoundPostGramException("File not found: " + postContent.FilePath);

            return new FileInfoModel(postContent.Name, postContent.MimeType, postContent.FilePath);
        }

        private bool FileExists(string filePath)
        {
            return !new FileInfo(filePath).Exists;
        }
    }
}
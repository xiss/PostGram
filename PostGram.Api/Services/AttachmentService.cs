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

        public void DeleteFile(Guid id)
        {
            FileInfo fileFile = new(Path.Combine(Directory.GetCurrentDirectory(), _appConfig.AttachesFolderName,
                    id.ToString()));
            if (!fileFile.Exists)
                throw new NotFoundPostGramException($"File {id} not found");
            try
            {
                fileFile.Delete();
            }
            catch (Exception e)
            {
                throw new FilePostGramException(e.Message, e);
            }
        }

        public void Dispose()
        {
            _dataContext.Dispose();
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

        public async Task<FileInfoModel> GetPostContent(Guid postContentId, Guid currentUserId)
        {
            PostContent? postContent = await _dataContext.PostContents
                .Include(pc => pc.Author)
                .ThenInclude(u => u.Masters)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == postContentId);
            if (postContent == null)
                throw new NotFoundPostGramException("PostContent not found in DB: " + postContentId);

            if (!FileExists(postContent.FilePath))
                throw new NotFoundPostGramException("File not found: " + postContentId);

            if (postContent.AuthorId != currentUserId &&
                !postContent.Author.Masters.Any(s => (s.SlaveId == currentUserId && s.Status)))
                throw new AuthorizationPostGramException("Access denied");

            return new FileInfoModel(postContent.Name, postContent.MimeType, postContent.FilePath);
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

        private bool FileExists(string filePath)
        {
            return new FileInfo(filePath).Exists;
        }
    }
}
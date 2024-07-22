using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PostGram.Api.Configs;
using PostGram.Common.Exceptions;
using PostGram.Common.Interfaces.Services;
using PostGram.Common.Models.Attachment;
using PostGram.DAL;
using PostGram.DAL.Entities;

namespace PostGram.BLL.Services
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

        public void Dispose()
        {
            _dataContext.Dispose();
        }

        public async Task<FileInfoModel> GetAvatarForUser(Guid userId)
        {
            Avatar? avatar = await _dataContext.Avatars.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId);
            if (avatar == null)
                throw new NotFoundPostGramException("Avatar not found in DB for user: " + userId);

            if (!CheckAttachmentsExists(avatar.Id.ToString()))
                throw new NotFoundPostGramException("File not found: " + avatar.Id);

            return new FileInfoModel() { MimeType = avatar.MimeType, Name = avatar.Name, Path = Path.Combine(_appConfig.AttachmentsFolderPath, avatar.Id.ToString()) };
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

            if (!CheckAttachmentsExists(postContent.Id.ToString()))
                throw new NotFoundPostGramException("File not found: " + postContentId);

            if (postContent.Author.IsPrivate && postContent.AuthorId != currentUserId &&
                !postContent.Author.Masters.Any(s => s.SlaveId == currentUserId && s.Status))
                throw new AuthorizationPostGramException("Access denied");

            return new FileInfoModel() { MimeType = postContent.MimeType, Name = postContent.Name, Path = Path.Combine(_appConfig.AttachmentsFolderPath, postContent.Id.ToString()) };
        }

        public async Task<MetadataModel> UploadFile(IFormFile file)
        {
            MetadataModel model = new()
            {
                TempId = Guid.NewGuid(),
                Name = file.FileName,
                MimeType = file.ContentType,
                Size = file.Length
            };
            string newPath = Path.Combine(Path.GetTempPath(), model.TempId.ToString());

            using (var stream = File.Create(newPath))
            {
                await file.CopyToAsync(stream);
            }

            return model;
        }

        private bool CheckAttachmentsExists(string attachmentName)
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
}
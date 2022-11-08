using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PostGram.Api.Configs;
using PostGram.Api.Models;
using PostGram.Common.Exceptions;
using PostGram.DAL;
using PostGram.DAL.Entities;

namespace PostGram.Api.Services
{
    public class AttachmentService : IDisposable, IAttachmentService
    {
        private readonly DataContext _dataContext;
        private readonly AppConfig _appConfig;
        private readonly IMapper _mapper;

        public AttachmentService(DataContext dataContext, IOptions<AppConfig> appConfig, IMapper mapper)
        {
            _dataContext = dataContext;
            _appConfig = appConfig.Value;
            _mapper = mapper;
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

        /// <summary>
        /// Save file in App directory
        /// </summary>
        /// <param name="temporaryFileId"></param>
        /// <returns>PathToFile</returns>
        /// <exception cref="AttachFileNotFoundPostGramException"></exception>
        /// <exception cref="AttachPostGramException"></exception>
        public async Task<string> ApplyFile(string temporaryFileId)
        {
            FileInfo tempFile = new(Path.Combine(Path.GetTempPath(), temporaryFileId));
            if (!tempFile.Exists)
                throw new AttachFileNotFoundPostGramException("File not found: " + tempFile.FullName);
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
                throw new AttachPostGramException(e.Message, e);
            }
        }

        public async Task<AttachmentModel> GetAvatarForUser(Guid userId)
        {
            Avatar? avatar = await _dataContext.Avatars.FirstOrDefaultAsync(x => x.UserId == userId);
            if (avatar == null)
                throw new AttachFileNotFoundPostGramException("Avatar not found in DB for user: " + userId);

            FileExists(avatar.FilePath);

            return _mapper.Map<AttachmentModel>(avatar);
        }

        public async Task<AttachmentModel> GetAttachment(Guid attachmentId)
        {
            Attachment? attachment = await _dataContext.Attachments.FirstOrDefaultAsync(a => a.Id == attachmentId);
            if (attachment == null)
                throw new AttachFileNotFoundPostGramException("Attachment not found in DB: " + attachmentId);

            FileExists(attachment.FilePath);

            return _mapper.Map<AttachmentModel>(attachment);
        }

        private bool FileExists(string filePath)
        {
            if (!new FileInfo(filePath).Exists)
                throw new AttachFileNotFoundPostGramException("File not found: " + filePath);

            return true;
        }
    }
}
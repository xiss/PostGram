using Microsoft.AspNetCore.Http;
using PostGram.Common.Dtos.Attachment;
using PostGram.Common.Interfaces.Base.Commands;
using PostGram.Common.Requests.Commands;

namespace PostGram.BLL.Handlers.Commands;

public class UploadAttachmentsHandler : ICommandHandler<UploadAttachmentsCommand>
{
    public async Task Execute(UploadAttachmentsCommand command)
    {
        // TODO попробовать использовать S3
        foreach (IFormFile attachment in command.AttachmentsToUpload)
        {
            MetadataDto model = new()
            {
                TempId = Guid.NewGuid(),
                Name = attachment.FileName,
                MimeType = attachment.ContentType,
                Size = attachment.Length
            };
            string newPath = Path.Combine(Path.GetTempPath(), model.TempId.ToString());

            await using FileStream stream = File.Create(newPath);
            await attachment.CopyToAsync(stream);
        }

    }
}
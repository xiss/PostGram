using Microsoft.AspNetCore.Http;
using PostGram.Common.Interfaces.Base.Commands;

namespace PostGram.Common.Requests.Commands;

public class UploadAttachmentsCommand : ICommand
{
    public List<IFormFile> AttachmentsToUpload { get; init; } = new();
}
using PostGram.Common.Constants;
using PostGram.Common.Dtos.Attachment;
using System.ComponentModel.DataAnnotations;
using PostGram.BLL.Interfaces.Base.Commands;

namespace PostGram.Common.Requests.Commands;

public record UpdatePostCommand : ICommand
{
    [Required]
    public Guid Id { get; init; }
    [StringLength(ModelValidation.PostHeaderLength)]
    public string? UpdatedHeader { get; init; } = null;
    [StringLength(ModelValidation.PostBodyLength)]
    public string? UpdatedBody { get; init; } = null;
    public ICollection<MetadataModel> NewContent { get; init; } = new List<MetadataModel>();
    public ICollection<AttachmentDto> ContentToDelete { get; init; } = new List<AttachmentDto>();
}
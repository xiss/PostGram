using System.ComponentModel.DataAnnotations;
using PostGram.Common.Constants;
using PostGram.Common.Dtos.Attachment;

namespace PostGram.Common.Requests;

public record CreatePostModel
{
    [Required]
    [StringLength(ModelValidation.PostHeaderLength)]
    public string Header { get; init; } = string.Empty;

    [Required]
    [StringLength(ModelValidation.PostBodyLength)]
    public string Body { get; init; } = string.Empty;

    [Required]
    public virtual ICollection<MetadataModel> Attachments { get; init; } = new List<MetadataModel>();
}
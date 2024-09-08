using System.ComponentModel.DataAnnotations;
using PostGram.Common.Dtos.Attachment;

namespace PostGram.Common.Requests;

public record AddAvatarRequestModel
{
    [Required]
    public MetadataModel Avatar { get; init; } = new();
    [Required]
    public Guid UserId { get; init; }
}
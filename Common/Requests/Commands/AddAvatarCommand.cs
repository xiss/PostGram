using PostGram.Common.Dtos.Attachment;
using PostGram.Common.Interfaces.Base.Commands;

using System.ComponentModel.DataAnnotations;

namespace PostGram.Common.Requests.Commands;

public record AddAvatarCommand : ICommand
{
    [Required]
    public MetadataModel Avatar { get; init; } = new();
    [Required]
    public Guid UserId { get; init; }
}
using PostGram.Common.Dtos.Attachment;
using System.ComponentModel.DataAnnotations;
using PostGram.BLL.Interfaces.Base.Commands;

namespace PostGram.Common.Requests.Commands;

public record AddAvatarCommand : ICommand
{
    [Required]
    public MetadataModel Avatar { get; init; } = new();
    [Required]
    public Guid UserId { get; init; }
}
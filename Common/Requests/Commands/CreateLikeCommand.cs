using PostGram.Common.Enums;
using System.ComponentModel.DataAnnotations;
using PostGram.Common.Interfaces.Base.Commands;

namespace PostGram.Common.Requests.Commands;

public record CreateLikeCommand : ICommand
{
    [Required]
    public bool IsLike { get; init; }

    [Required]
    public Guid EntityId { get; init; }

    [Required]
    public LikableEntities EntityType { get; init; }
}
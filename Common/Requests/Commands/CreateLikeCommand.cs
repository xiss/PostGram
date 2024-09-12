using PostGram.Common.Enums;
using PostGram.Common.Interfaces.Base.Commands;

using System.ComponentModel.DataAnnotations;

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
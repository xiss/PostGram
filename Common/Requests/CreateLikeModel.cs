using System.ComponentModel.DataAnnotations;
using PostGram.Common.Enums;

namespace PostGram.Common.Requests;

public record CreateLikeModel
{
    [Required]
    public bool IsLike { get; init; }

    [Required]
    public Guid EntityId { get; init; }

    [Required]
    public LikableEntities EntityType { get; init; }
}
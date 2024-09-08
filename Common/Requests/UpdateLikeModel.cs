using System.ComponentModel.DataAnnotations;

namespace PostGram.Common.Requests;

public record UpdateLikeModel
{
    [Required]
    public Guid Id { get; init; }

    public bool? IsLike { get; init; }
}
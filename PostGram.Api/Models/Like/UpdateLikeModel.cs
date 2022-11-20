using System.ComponentModel.DataAnnotations;

namespace PostGram.Api.Models.Like
{
    public record UpdateLikeModel
    {
        [Required]
        public Guid Id { get; init; }

        public bool? IsLike { get; init; }
    }
}

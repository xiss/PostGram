using PostGram.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace PostGram.Api.Models.Like
{
    public record CreateLikeModel
    {
        [Required]
        public bool IsLike { get; init; }

        [Required]
        public Guid EntityId { get; init; }

        [Required]
        public LikableEntities EntityType { get; init; }
    }
}
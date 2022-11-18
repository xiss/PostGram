using PostGram.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace PostGram.Api.Models.Like
{
    public class CreateLikeModel
    {
        [Required]
        public bool IsLike { get; set; }

        [Required]
        public Guid EntityId { get; set; }

        [Required]
        public LikableEntities EntityType { get; set; }
    }
}
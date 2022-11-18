using System.ComponentModel.DataAnnotations;

namespace PostGram.Api.Models.Like
{
    public class UpdateLikeModel
    {
        [Required]
        public Guid Id { get; set; }

        public bool? IsLike { get; set; }
    }
}

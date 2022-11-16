using System.ComponentModel.DataAnnotations;

namespace PostGram.Api.Models.Comment
{
    public class UpdateCommentModel
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string NewBody { get; set; } = null!;
    }
}
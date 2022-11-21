using PostGram.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace PostGram.Api.Models.Comment
{
    public record UpdateCommentModel
    {
        [Required]
        public Guid Id { get; init; }
        [Required]
        [StringLength(ModelValidation.CommentLength)]
        public string NewBody { get; init; } = string.Empty;
    }
}
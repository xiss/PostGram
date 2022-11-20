using System.ComponentModel.DataAnnotations;

namespace PostGram.Api.Models.Comment
{
    public record CreateCommentModel
    {
        [Required]
        public string Body { get; init; } =string.Empty;
        [Required]
        public Guid PostId { get; init; }
        public Guid? QuotedCommentId { get; init; } = null;
        public string? QuotedText { get; set; } = null;
    }
}
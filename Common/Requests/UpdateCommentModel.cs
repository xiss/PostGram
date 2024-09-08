using System.ComponentModel.DataAnnotations;
using PostGram.Common.Constants;

namespace PostGram.Common.Requests;

public record UpdateCommentModel
{
    [Required]
    public Guid Id { get; init; }
    [Required]
    [StringLength(ModelValidation.CommentLength)]
    public string NewBody { get; init; } = string.Empty;
}
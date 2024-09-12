using PostGram.Common.Constants;
using PostGram.Common.Interfaces.Base.Commands;
using System.ComponentModel.DataAnnotations;

namespace PostGram.Common.Requests.Commands;

public record UpdateCommentCommand : ICommand
{
    [Required]
    public Guid Id { get; init; }
    [Required]
    [StringLength(ModelValidation.CommentLength)]
    public string NewBody { get; init; } = string.Empty;
}
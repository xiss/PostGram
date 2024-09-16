using PostGram.Common.Constants;
using System.ComponentModel.DataAnnotations;
using PostGram.Common.Interfaces.Base.Commands;

namespace PostGram.Common.Requests.Commands;

public record UpdateCommentCommand : ICommand
{
    [Required]
    public Guid Id { get; init; }
    [Required]
    [StringLength(ModelValidation.CommentLength)]
    public string NewBody { get; init; } = string.Empty;
}
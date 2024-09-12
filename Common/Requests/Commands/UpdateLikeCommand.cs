using PostGram.Common.Interfaces.Base.Commands;

using System.ComponentModel.DataAnnotations;

namespace PostGram.Common.Requests.Commands;

public record UpdateLikeCommand : ICommand
{
    [Required]
    public Guid Id { get; init; }

    public bool? IsLike { get; init; }
}
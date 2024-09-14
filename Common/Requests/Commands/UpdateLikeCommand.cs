using System.ComponentModel.DataAnnotations;
using PostGram.BLL.Interfaces.Base.Commands;

namespace PostGram.Common.Requests.Commands;

public record UpdateLikeCommand : ICommand
{
    [Required]
    public Guid Id { get; init; }

    public bool? IsLike { get; init; }
}
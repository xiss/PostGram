using PostGram.Common.Interfaces.Base.Commands;
using System.ComponentModel.DataAnnotations;

namespace PostGram.Common.Requests.Commands;

public record CreateSubscriptionCommand : ICommand
{
    [Required]
    public Guid MasterId { get; init; }
}
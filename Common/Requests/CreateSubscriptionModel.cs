using System.ComponentModel.DataAnnotations;

namespace PostGram.Common.Requests;

public record CreateSubscriptionModel
{
    [Required]
    public Guid MasterId { get; init; }
}
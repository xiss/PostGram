using System.ComponentModel.DataAnnotations;

namespace PostGram.Api.Models.Subscription
{
    public record CreateSubscriptionModel
    {
        [Required]
        public Guid MasterId { get; init; }
    }
}

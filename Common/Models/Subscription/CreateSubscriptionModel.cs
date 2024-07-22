using System.ComponentModel.DataAnnotations;

namespace PostGram.Common.Models.Subscription
{
    public record CreateSubscriptionModel
    {
        [Required]
        public Guid MasterId { get; init; }
    }
}

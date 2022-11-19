using System.ComponentModel.DataAnnotations;

namespace PostGram.Api.Models.Subscription
{
    public class CreateSubscriptionModel
    {
        [Required]
        public Guid MasterId { get; set; }
    }
}

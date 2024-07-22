namespace PostGram.Common.Models.Subscription
{
    public record UpdateSubscriptionModel
    {
        public Guid Id { get; init; }
        public bool Status { get; init; } 
    }
}

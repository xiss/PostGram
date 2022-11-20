namespace PostGram.Api.Models.Subscription
{
    public record SubscriptionModel
    {
        public Guid Id { get; init; }
        public Guid SlaveId { get; init; }
        public Guid MasterId { get; init; }
        public bool Status { get; init; } 
        public DateTimeOffset Created { get; init; }
        public DateTimeOffset? Edited { get; init; }
    }
}

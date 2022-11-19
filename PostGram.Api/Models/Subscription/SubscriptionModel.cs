namespace PostGram.Api.Models.Subscription
{
    public class SubscriptionModel
    {
        public Guid Id { get; set; }
        public Guid SlaveId { get; set; }
        public Guid MasterId { get; set; }
        public bool Status { get; set; } 
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Edited { get; set; }
    }
}

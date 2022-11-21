namespace PostGram.DAL.Entities
{
    public class Subscription
    {
        public Guid Id { get; set; }
        public Guid SlaveId { get; set; }
        public virtual User Slave { get; set; } = null!;
        public Guid MasterId { get; set; }
        public virtual User Master { get; set; } = null!;
        public bool Status { get; set; } = false;
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Edited { get; set; }
    }
}
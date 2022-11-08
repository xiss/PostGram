namespace PostGram.DAL.Entities
{
    public class Avatar : Attachment
    {
        public Guid UserId { get; set; }
        public virtual User User { get; set; } = null!;
    }
}
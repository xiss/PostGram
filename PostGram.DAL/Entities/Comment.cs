namespace PostGram.DAL.Entities
{
    public class Comment : CreationBase
    {
        public string Body { get; set; } = null!;
        public DateTimeOffset? Edited { get; set; }
        public bool IsDeleted { get; set; } = false;
        public Guid PostId { get; set; }
        public Post Post { get; set; } = null!;
    }
}

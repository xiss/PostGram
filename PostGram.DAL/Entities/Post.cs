namespace PostGram.DAL.Entities
{
    public class Post : CreationBase
    {
        public string Header { get; set; } = null!;
        public string Body { get; set; } = null!;
        public DateTimeOffset? Edited { get; set; }
        public bool IsDeleted { get; set; } = false;
        public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}

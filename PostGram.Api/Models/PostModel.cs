namespace PostGram.Api.Models
{
    public class PostModel
    {
        public Guid Id { get; set; }
        public Guid AuthorId { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Edited { get; set; }
        public string Header { get; set; } = null!;
        public string Body { get; set; } = null!;
        public virtual string[] Attachments { get; set; } = null!;
        public virtual CommentModel[]? Comments { get; set; }
    }
}
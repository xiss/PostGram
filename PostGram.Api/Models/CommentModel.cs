namespace PostGram.Api.Models
{
    public class CommentModel
    {
        public Guid Id { get; set; }
        public Guid AuthorId { get; set; }
        public Guid PostId { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Edited { get; set; }
        public string Body { get; set; } = null!;
    }
}
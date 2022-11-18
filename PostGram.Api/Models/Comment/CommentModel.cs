namespace PostGram.Api.Models.Comment
{
    public class CommentModel
    {
        public Guid Id { get; set; }
        public Guid AuthorId { get; set; }
        public Guid PostId { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Edited { get; set; }
        public string Body { get; set; } = String.Empty;
        public int LikeCount { get; set; }
        public int DislikeCount { get; set; }
    }
}
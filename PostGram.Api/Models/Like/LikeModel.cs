namespace PostGram.Api.Models.Like
{
    public class LikeModel
    {
        public Guid Id { get; set; }
        public bool? IsLike { get; set; }
        public Guid? CommentId { get; set; }
        public Guid? PostId { get; set; }
        public Guid AuthorId { get; set; }
        public DateTimeOffset Created { get; set; }
    }
}

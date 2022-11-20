using PostGram.Api.Models.User;

namespace PostGram.Api.Models.Comment
{
    public record CommentModel
    {
        public Guid Id { get; init; }
        public UserModel Author { get; init; }
        public Guid PostId { get; init; }
        public DateTimeOffset Created { get; init; }
        public DateTimeOffset? Edited { get; init; }
        public string Body { get; init; } = String.Empty;
        public int LikeCount { get; init; }
        public int DislikeCount { get; init; }
    }
}
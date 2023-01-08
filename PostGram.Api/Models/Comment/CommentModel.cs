using PostGram.Api.Models.Like;
using PostGram.Api.Models.User;

namespace PostGram.Api.Models.Comment
{
    public record CommentModel
    {
        public Guid Id { get; init; }
        public UserModel Author { get; init; } = new();
        public Guid PostId { get; init; }
        public DateTimeOffset Created { get; init; }
        public DateTimeOffset? Edited { get; init; }
        public string Body { get; init; } = string.Empty;
        public int LikeCount { get; init; }
        public int DislikeCount { get; init; }
        public Guid? QuotedCommentId { get; init; }
        public string? QuotedText { get; init; }
        public  LikeModel? LikeByUser { get; set; }
    }
}
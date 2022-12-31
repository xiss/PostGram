using PostGram.Api.Models.Attachment;
using PostGram.Api.Models.Comment;
using PostGram.Api.Models.User;

namespace PostGram.Api.Models.Post
{
    public record PostModel
    {
        public UserModel Author { get; init; } = new UserModel();
        public Guid Id { get; init; }
        public DateTimeOffset Created { get; init; }
        public DateTimeOffset? Edited { get; init; }
        public string Header { get; init; } = string.Empty;
        public string Body { get; init; } = string.Empty;
        public int LikeCount { get; init; }
        public int DislikeCount { get; init; }
        public int CommentCount { get; init; }
        public bool? IsLikedByUser { get; set; }
        public ICollection<AttachmentModel> Content { get; init; } = new List<AttachmentModel>();
    }
}
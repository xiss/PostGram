using PostGram.Api.Models.Attachment;
using PostGram.Api.Models.Comment;
using PostGram.Api.Models.User;

namespace PostGram.Api.Models.Post
{
    public class PostModel 
    {
        public UserModel Author { get; set; } = new UserModel();
        public Guid Id { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Edited { get; set; }
        public string Header { get; set; } = null!;
        public string Body { get; set; } = null!;
        public CommentModel[]? Comments { get; set; }
        public ICollection<AttachmentModel> Content { get; set; } = new List<AttachmentModel>();
    }
}
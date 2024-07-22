using PostGram.Common.Models.Comment;
using PostGram.Common.Models.Like;
using PostGram.Common.Models.Post;

namespace PostGram.Common.Interfaces.Services
{
    public interface IPostService
    {
        Task<Guid> CreateComment(CreateCommentModel model, Guid currentUserId);

        Task<Guid> CreateLike(CreateLikeModel model, Guid currentUserId);

        Task<Guid> CreatePost(CreatePostModel model, Guid currentUserId);

        Task<Guid> DeleteComment(Guid commentId, Guid currentUserId);

        Task<Guid> DeletePost(Guid postId, Guid currentUserId);

        Task<CommentModel> GetComment(Guid commentId, Guid currentUserId);

        Task<CommentModel[]> GetCommentsForPost(Guid postId, Guid currentUserId);

        Task<PostModel> GetPost(Guid postId, Guid currentUserId);

        Task<List<PostModel>> GetPosts(int takeAmount, int skipAmount, Guid currentUserId);

        Task<CommentModel> UpdateComment(UpdateCommentModel model, Guid currentUserId);

        Task<LikeModel> UpdateLike(UpdateLikeModel model, Guid currentUserId);

        Task<PostModel> UpdatePost(UpdatePostModel model, Guid currentUserId);
    }
}
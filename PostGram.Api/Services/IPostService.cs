using PostGram.Api.Models.Comment;
using PostGram.Api.Models.Like;
using PostGram.Api.Models.Post;

namespace PostGram.Api.Services
{
    public interface IPostService
    {
        Task<Guid> CreateComment(CreateCommentModel model, Guid currentUserId);

        Task<Guid> CreateLike(CreateLikeModel model, Guid currentUserId);

        Task<Guid> CreatePost(CreatePostModel model, Guid currentUserId);

        Task<Guid> DeleteComment(Guid commentId, Guid currentUserId);

        Task<Guid> DeletePost(Guid postId, Guid currentUserId);

        Task<CommentModel> GetComment(Guid commentId);

        Task<CommentModel[]> GetCommentsForPost(Guid postId);

        Task<PostModel> GetPost(Guid postId, Guid currentUserId);

        Task<List<PostModel>> GetPosts(int takeAmount, int skipAmount, Guid currentUserId);

        Task<CommentModel> UpdateComment(UpdateCommentModel model, Guid currentUserId);

        Task<LikeModel> UpdateLike(UpdateLikeModel model, Guid currentUserId);

        Task<PostModel> UpdatePost(UpdatePostModel model, Guid currentUserId);
    }
}
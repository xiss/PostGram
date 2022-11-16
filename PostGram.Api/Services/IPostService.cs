using PostGram.Api.Models.Comment;
using PostGram.Api.Models.Like;
using PostGram.Api.Models.Post;

namespace PostGram.Api.Services
{
    public interface IPostService
    {
        Task<bool> CheckPostExist(Guid postId);

        Task<Guid> CreatePost(CreatePostModel model, Guid userId);

        Task DeletePost(Guid postId, Guid userId);

        Task<PostModel> GetPost(Guid postId);

        Task<List<PostModel>> GetPosts(int take, int skip);

        Task<PostModel> UpdatePost(UpdatePostModel model, Guid curentUserId);

        Task<bool> CheckCommentExist(Guid commentId);

        Task<Guid> CreateComment(CreateCommentModel model, Guid userId);

        Task<Guid> DeleteComment(Guid commentId);

        Task<CommentModel> GetComment(Guid commentId);

        Task<CommentModel[]> GetCommentsForPost(Guid postId);

        Task<CommentModel> UpdateComment(UpdateCommentModel model);
        Task<Guid> CreateLike(CreateLikeModel model, Guid curentUserId);
    }
}
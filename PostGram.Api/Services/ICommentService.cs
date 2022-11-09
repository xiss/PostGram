using PostGram.Api.Models.Comment;

namespace PostGram.Api.Services
{
    public interface ICommentService
    {
        Task<bool> CheckCommentExist(Guid commentId);

        Task<Guid> CreateComment(CreateCommentModel model, Guid userId);

        Task<Guid> DeleteComment(Guid commentId);

        Task<CommentModel> GetComment(Guid commentId);

        Task<CommentModel[]> GetCommentsForPost(Guid postId);

        Task<CommentModel> UpdateComment(UpdateCommentModel model);
    }
}
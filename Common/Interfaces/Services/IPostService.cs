using PostGram.Common.Dtos.Comment;
using PostGram.Common.Dtos.Post;
using PostGram.Common.Requests;

namespace PostGram.Common.Interfaces.Services;

public interface IPostService
{
    Task CreateComment(CreateCommentModel model, Guid currentUserId);

    Task CreateLike(CreateLikeModel model, Guid currentUserId);

    Task CreatePost(CreatePostModel model, Guid currentUserId);

    Task DeleteComment(Guid commentId, Guid currentUserId);

    Task DeletePost(Guid postId, Guid currentUserId);

    Task<CommentDto> GetComment(Guid commentId, Guid currentUserId);

    Task<CommentDto[]> GetCommentsForPost(Guid postId, Guid currentUserId);

    Task<PostDto> GetPost(Guid postId, Guid currentUserId);

    Task<List<PostDto>> GetPosts(int takeAmount, int skipAmount, Guid currentUserId);

    Task UpdateComment(UpdateCommentModel model, Guid currentUserId);

    Task UpdateLike(UpdateLikeModel model, Guid currentUserId);

    Task UpdatePost(UpdatePostModel model, Guid currentUserId);
}
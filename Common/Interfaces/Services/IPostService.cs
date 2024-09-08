using PostGram.Common.Dtos.Comment;
using PostGram.Common.Dtos.Like;
using PostGram.Common.Dtos.Post;
using PostGram.Common.Requests;

namespace PostGram.Common.Interfaces.Services;

public interface IPostService
{
    Task<Guid> CreateComment(CreateCommentModel model, Guid currentUserId);

    Task<Guid> CreateLike(CreateLikeModel model, Guid currentUserId);

    Task<Guid> CreatePost(CreatePostModel model, Guid currentUserId);

    Task<Guid> DeleteComment(Guid commentId, Guid currentUserId);

    Task<Guid> DeletePost(Guid postId, Guid currentUserId);

    Task<CommentDto> GetComment(Guid commentId, Guid currentUserId);

    Task<CommentDto[]> GetCommentsForPost(Guid postId, Guid currentUserId);

    Task<PostDto> GetPost(Guid postId, Guid currentUserId);

    Task<List<PostDto>> GetPosts(int takeAmount, int skipAmount, Guid currentUserId);

    Task<CommentDto> UpdateComment(UpdateCommentModel model, Guid currentUserId);

    Task<LikeDto> UpdateLike(UpdateLikeModel model, Guid currentUserId);

    Task<PostDto> UpdatePost(UpdatePostModel model, Guid currentUserId);
}
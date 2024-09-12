using PostGram.Common.Requests.Commands;
using PostGram.Common.Requests.Queries;
using PostGram.Common.Results;

namespace PostGram.Common.Interfaces.Services;

public interface IPostService
{
    Task CreateComment(CreateCommentCommand command, Guid currentUserId);

    Task CreateLike(CreateLikeCommand command, Guid currentUserId);

    Task CreatePost(CreatePostCommand command, Guid currentUserId);

    Task DeleteComment(DeleteCommentCommand command, Guid currentUserId);

    Task DeletePost(DeletePostCommand command, Guid currentUserId);

    Task<GetCommentResult> GetComment(GetCommentQuery query, Guid currentUserId);

    Task<GetCommentsForPostResult> GetCommentsForPost(GetCommentsForPostQuery query, Guid currentUserId);

    Task<GetPostResult> GetPost(GetPostQuery query, Guid currentUserId);

    Task<GetPostsResult> GetPosts(GetPostsQuery query, Guid currentUserId);

    Task UpdateComment(UpdateCommentCommand command, Guid currentUserId);

    Task UpdateLike(UpdateLikeCommand command, Guid currentUserId);

    Task UpdatePost(UpdatePostCommand command, Guid currentUserId);
}
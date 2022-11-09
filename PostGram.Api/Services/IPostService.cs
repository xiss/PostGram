using PostGram.Api.Models.Post;

namespace PostGram.Api.Services
{
    public interface IPostService
    {
        Task<bool> CheckPostExist(Guid postId);

        Task<Guid> CreatePost(CreatePostModel model, Guid userId);

        Task<PostModel> GetPost(Guid postId);
    }
}
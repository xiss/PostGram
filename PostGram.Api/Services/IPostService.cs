using Microsoft.EntityFrameworkCore;
using PostGram.Api.Models;
using PostGram.DAL;
using PostGram.DAL.Entities;

namespace PostGram.Api.Services
{
    public interface IPostService
    {
        Task<bool> CheckPostExist(Guid postId);
        Task<Guid> CreatePost(CreatePostModel model, Guid userId);
        Task<PostModel> GetPost(Guid postId);
    }
}
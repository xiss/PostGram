using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostGram.Api.Helpers;
using PostGram.Common.Dtos.Attachment;
using PostGram.Common.Dtos.Comment;
using PostGram.Common.Dtos.Like;
using PostGram.Common.Dtos.Post;
using PostGram.Common.Dtos.User;
using PostGram.Common.Interfaces.Services;
using PostGram.Common.Requests;

namespace PostGram.Api.Controllers;

[ApiExplorerSettings(GroupName = Common.Constants.Api.EndpointApiName)]
[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
public class PostController : ControllerBase
{
    private readonly IPostService _postService;

    public PostController(IPostService postService)
    {
        _postService = postService;
    }

    [HttpPost]
    public async Task CreateComment(CreateCommentModel model)
    {
        await _postService.CreateComment(model, this.GetCurrentUserId());
    }

    [HttpPost]
    public async Task CreateLike(CreateLikeModel model)
    {
        await _postService.CreateLike(model, this.GetCurrentUserId());
    }

    [HttpPost]
    public async Task CreatePost(CreatePostModel model)
    {
        await _postService.CreatePost(model, this.GetCurrentUserId());
    }

    [HttpDelete]
    public async Task DeleteComment(Guid commentId)
    {
        await _postService.DeleteComment(commentId, this.GetCurrentUserId());
    }

    [HttpDelete]
    public async Task DeletePost(Guid postId)
    {
        await _postService.DeletePost(postId, this.GetCurrentUserId());
    }

    [HttpGet]
    public async Task<CommentDto> GetComment(Guid commentId)
    {
        return await _postService.GetComment(commentId, this.GetCurrentUserId());
    }

    [HttpGet]
    public async Task<CommentDto[]> GetCommentsForPost(Guid postId)
    {
        return await _postService.GetCommentsForPost(postId, this.GetCurrentUserId());
    }

    [HttpGet]
    public async Task<PostDto> GetPost(Guid postId)
    {
        return await _postService.GetPost(postId, this.GetCurrentUserId());
    }

    [HttpGet]
    public async Task<List<PostDto>> GetPosts(int take = 10, int skip = 0)
    {
        return await _postService.GetPosts(take, skip, this.GetCurrentUserId());
    }

    [HttpPut]
    public async Task UpdateComment(UpdateCommentModel model)
    {
        await _postService.UpdateComment(model, this.GetCurrentUserId());
    }

    [HttpPut]
    public async Task UpdateLike(UpdateLikeModel model)
    {
        await _postService.UpdateLike(model, this.GetCurrentUserId());
    }

    [HttpPut]
    public async Task UpdatePost(UpdatePostModel model)
    {
        await _postService.UpdatePost(model, this.GetCurrentUserId());
    }

    //private void AddAvatarLink(UserDto model)
    //{
    //    //if (model.Avatar != null)
    //    //    model.Avatar.Link = AttachmentController.GetLinkForAvatar(Url, model.Id);
    //}

    //private void AddPostContentLink(AttachmentDto model)
    //{
    //    //model.Link = AttachmentController.GetLinkForPostContent(Url, model.Id);
    //}

    //TODO ужимать картинки и делать 2 версии разного размера
}
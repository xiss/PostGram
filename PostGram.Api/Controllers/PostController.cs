using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostGram.Api.Helpers;
using PostGram.Common.Interfaces.Services;
using PostGram.Common.Requests.Commands;
using PostGram.Common.Requests.Queries;
using PostGram.Common.Results;

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
    public async Task CreateComment(CreateCommentCommand command)
    {
        await _postService.CreateComment(command, this.GetCurrentUserId());
    }

    [HttpPost]
    public async Task CreateLike(CreateLikeCommand command)
    {
        await _postService.CreateLike(command, this.GetCurrentUserId());
    }

    [HttpPost]
    public async Task CreatePost(CreatePostCommand command)
    {
        await _postService.CreatePost(command, this.GetCurrentUserId());
    }

    [HttpDelete]
    public async Task DeleteComment(DeleteCommentCommand command)
    {
        await _postService.DeleteComment(command, this.GetCurrentUserId());
    }

    [HttpDelete]
    public async Task DeletePost(DeletePostCommand command)
    {
        await _postService.DeletePost(command, this.GetCurrentUserId());
    }

    [HttpGet]
    public async Task<GetCommentResult> GetComment(GetCommentQuery query)
    {
        return await _postService.GetComment(query, this.GetCurrentUserId());
    }

    [HttpGet]
    public async Task<GetCommentsForPostResult> GetCommentsForPost(GetCommentsForPostQuery query)
    {
        return await _postService.GetCommentsForPost(query, this.GetCurrentUserId());
    }

    [HttpGet]
    public async Task<GetPostResult> GetPost(GetPostQuery query)
    {
        return await _postService.GetPost(query, this.GetCurrentUserId());
    }

    [HttpGet]
    public async Task<GetPostsResult> GetPosts(GetPostsQuery query)
    {
        return await _postService.GetPosts(query, this.GetCurrentUserId());
    }

    [HttpPut]
    public async Task UpdateComment(UpdateCommentCommand command)
    {
        await _postService.UpdateComment(command, this.GetCurrentUserId());
    }

    [HttpPut]
    public async Task UpdateLike(UpdateLikeCommand command)
    {
        await _postService.UpdateLike(command, this.GetCurrentUserId());
    }

    [HttpPut]
    public async Task UpdatePost(UpdatePostCommand command)
    {
        await _postService.UpdatePost(command, this.GetCurrentUserId());
    }

    //TODO ужимать картинки и делать 2 версии разного размера
}
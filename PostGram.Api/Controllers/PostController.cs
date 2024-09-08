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
    public async Task<Guid> CreateComment(CreateCommentModel model)
    {
        return await _postService.CreateComment(model, this.GetCurrentUserId());
    }

    [HttpPost]
    public async Task<Guid> CreateLike(CreateLikeModel model)
    {
        return await _postService.CreateLike(model, this.GetCurrentUserId());
    }

    [HttpPost]
    public async Task<Guid> CreatePost(CreatePostModel model)
    {
        return await _postService.CreatePost(model, this.GetCurrentUserId());
    }

    [HttpDelete]
    public async Task<Guid> DeleteComment(Guid commentId)
    {
        return await _postService.DeleteComment(commentId, this.GetCurrentUserId());
    }

    [HttpDelete]
    public async Task<Guid> DeletePost(Guid postId)
    {
        await _postService.DeletePost(postId, this.GetCurrentUserId());
        return postId;
    }

    [HttpGet]
    public async Task<CommentDto> GetComment(Guid commentId)
    {
        CommentDto model = await _postService.GetComment(commentId, this.GetCurrentUserId());
        AddAvatarLink(model.Author);
        return model;
    }

    [HttpGet]
    public async Task<CommentDto[]> GetCommentsForPost(Guid postId)
    {
        CommentDto[] model = await _postService.GetCommentsForPost(postId, this.GetCurrentUserId());
        foreach (CommentDto comment in model)
        {
            AddAvatarLink(comment.Author);
        }

        return model;
    }

    [HttpGet]
    public async Task<PostDto> GetPost(Guid postId)
    {
        PostDto model = await _postService.GetPost(postId, this.GetCurrentUserId());
        AddAvatarLink(model.Author);
        foreach (AttachmentDto attachment in model.Content)
        {
            AddPostContentLink(attachment);
        }
        return model;
    }

    [HttpGet]
    public async Task<List<PostDto>> GetPosts(int take = 10, int skip = 0)
    {
        List<PostDto> models = await _postService.GetPosts(take, skip, this.GetCurrentUserId());
        foreach (PostDto model in models)
        {
            AddAvatarLink(model.Author);
            foreach (AttachmentDto attachment in model.Content)
            {
                AddPostContentLink(attachment);
            }
        }
        return models;
    }

    [HttpPut]
    public async Task<CommentDto> UpdateComment(UpdateCommentModel model)
    {
        CommentDto commentModel = await _postService.UpdateComment(model, this.GetCurrentUserId());
        AddAvatarLink(commentModel.Author);
        return commentModel;
    }

    [HttpPut]
    public async Task<LikeDto> UpdateLike(UpdateLikeModel model)
    {
        return await _postService.UpdateLike(model, this.GetCurrentUserId());
    }

    [HttpPut]
    public async Task<PostDto> UpdatePost(UpdatePostModel model)
    {
        PostDto postModel = await _postService.UpdatePost(model, this.GetCurrentUserId());
        AddAvatarLink(postModel.Author);
        foreach (AttachmentDto attachment in postModel.Content)
        {
            AddPostContentLink(attachment);
        }
        return postModel;
    }

    private void AddAvatarLink(UserDto model)
    {
        //if (model.Avatar != null)
        //    model.Avatar.Link = AttachmentController.GetLinkForAvatar(Url, model.Id);
    }

    private void AddPostContentLink(AttachmentDto model)
    {
        //model.Link = AttachmentController.GetLinkForPostContent(Url, model.Id);
    }

    //TODO ужимать картинки и делать 2 версии разного размера
}
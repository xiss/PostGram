using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostGram.Api.Helpers;
using PostGram.Api.Models.Attachment;
using PostGram.Api.Models.Comment;
using PostGram.Api.Models.Like;
using PostGram.Api.Models.Post;
using PostGram.Api.Models.User;
using PostGram.Api.Services;

namespace PostGram.Api.Controllers
{
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
        public async Task<CommentModel> GetComment(Guid commentId)
        {
            CommentModel model = await _postService.GetComment(commentId);
            AddAvatarLink(model.Author);
            return model;
        }

        [HttpGet]
        public async Task<CommentModel[]> GetCommentsForPost(Guid postId)
        {
            CommentModel[] model = await _postService.GetCommentsForPost(postId);
            foreach (CommentModel comment in model)
            {
                AddAvatarLink(comment.Author);
            }

            return model;
        }

        [HttpGet]
        public async Task<PostModel> GetPost(Guid postId)
        {
            PostModel model = await _postService.GetPost(postId, this.GetCurrentUserId());
            AddAvatarLink(model.Author);
            foreach (AttachmentModel attachment in model.Content)
            {
                AddPostContentLink(attachment);
            }
            return model;
        }

        [HttpGet]
        public async Task<List<PostModel>> GetPosts(int take = 10, int skip = 0)
        {
            List<PostModel> models = await _postService.GetPosts(take, skip, this.GetCurrentUserId());
            foreach (PostModel model in models)
            {
                AddAvatarLink(model.Author);
                foreach (AttachmentModel attachment in model.Content)
                {
                    AddPostContentLink(attachment);
                }
            }
            return models;
        }

        [HttpPut]
        public async Task<CommentModel> UpdateComment(UpdateCommentModel model)
        {
            return await _postService.UpdateComment(model, this.GetCurrentUserId());
        }

        [HttpPut]
        public async Task<LikeModel> UpdateLike(UpdateLikeModel model)
        {
            return await _postService.UpdateLike(model, this.GetCurrentUserId());
        }

        [HttpPut]
        public async Task<PostModel> UpdatePost(UpdatePostModel model)
        {
            return await _postService.UpdatePost(model, this.GetCurrentUserId());
        }

        private void AddAvatarLink(UserModel model)
        {
            if (model.Avatar != null)
                model.Avatar.Link = AttachmentController.GetLinkForAvatar(Url, model.Id);
        }

        private void AddPostContentLink(AttachmentModel model)
        {
            model.Link = AttachmentController.GetLinkForPostContent(Url, model.Id);
        }

        //TODO DDOS
        //TODO ужимать картинки и делать 2 версии разного размера
    }
}
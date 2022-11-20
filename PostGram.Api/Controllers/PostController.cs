using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostGram.Api.Helpers;
using PostGram.Api.Models.Attachment;
using PostGram.Api.Models.Comment;
using PostGram.Api.Models.Like;
using PostGram.Api.Models.Post;
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
            if (model.Author.Avatar != null)
                model.Author.Avatar.Link = AttachmentController.GetLinkForAvatar(Url, model.Author.Id);
            return model;
        }

        [HttpGet]
        public async Task<CommentModel[]> GetCommentsForPost(Guid postId)
        {
            CommentModel[] model = await _postService.GetCommentsForPost(postId);
            foreach (CommentModel comment in model)
            {
                if (comment.Author.Avatar != null)
                    comment.Author.Avatar.Link = AttachmentController.GetLinkForAvatar(Url, comment.Author.Id);
            }

            return model;
        }

        [HttpGet]
        public async Task<PostModel> GetPost(Guid postId)
        {
            PostModel model = await _postService.GetPost(postId, this.GetCurrentUserId());
            if (model.Author.Avatar != null)
                model.Author.Avatar.Link = AttachmentController.GetLinkForAvatar(Url, model.Author.Id);
            foreach (AttachmentModel attachment in model.Content)
            {
                attachment.Link = AttachmentController.GetLinkForPostContent(Url, attachment.Id);
            }
            return model;
        }

        [HttpGet]
        public async Task<List<PostModel>> GetPosts(int take = 10, int skip = 0)
        {
            List<PostModel> models = await _postService.GetPosts(take, skip, this.GetCurrentUserId());
            foreach (PostModel model in models)
            {
                if (model.Author.Avatar != null)
                    model.Author.Avatar.Link = AttachmentController.GetLinkForAvatar(Url, model.Author.Id);
                foreach (AttachmentModel attachment in model.Content)
                {
                    attachment.Link = AttachmentController.GetLinkForPostContent(Url, attachment.Id);
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

        //TODO DDOS
        //TODO Модели в Рекордс
        //Также с коментами и постами, насчет лайков не знаю.
        //TODO Нужно ли удалять лайки если удаляем пост или комент?
    }
}
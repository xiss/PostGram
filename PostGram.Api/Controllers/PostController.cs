﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostGram.Api.Helpers;
using PostGram.Api.Models.Attachment;
using PostGram.Api.Models.Comment;
using PostGram.Api.Models.Post;
using PostGram.Api.Models.Like;
using PostGram.Api.Services;
using PostGram.Common.Exceptions;

namespace PostGram.Api.Controllers
{
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
        public async Task<Guid> CreatePost(CreatePostModel model)
        {
            Guid postId = await _postService.CreatePost(model, this.GetCurrentUserId());
            return postId;
        }

        [HttpGet]
        public async Task<PostModel> GetPost(Guid postId)
        {
            PostModel model = await _postService.GetPost(postId);
            model.Author.Avatar.Link = AttachmentController.GetLinkForAvatar(Url, model.Author.Id);
            foreach (AttachmentModel attachment in model.Content)
            {
                attachment.Link = AttachmentController.GetLinkForPostContent(Url, attachment.Id);
            }
            return model;
        }

        [HttpGet]
        public async Task<List<PostModel>> GetPosts(int take, int skip)
        {
            List<PostModel> models = await _postService.GetPosts(take, skip);
            foreach (PostModel model in models)
            {
                model.Author.Avatar.Link = AttachmentController.GetLinkForAvatar(Url, model.Author.Id);
                foreach (AttachmentModel attachment in model.Content)
                {
                    attachment.Link = AttachmentController.GetLinkForPostContent(Url, attachment.Id);
                }
            }
            return models;
        }

        [HttpDelete]
        public async Task<Guid> DeletePost(Guid postId)
        {
            await _postService.DeletePost(postId, this.GetCurrentUserId());
            return postId;
        }

        [HttpPut]
        public async Task<PostModel> UpdatePost(UpdatePostModel model)
        {
            return await _postService.UpdatePost(model, this.GetCurrentUserId());
        }

        [HttpPost]
        public async Task<Guid> CreateComment(CreateCommentModel model)
        {
            Guid userId = this.GetCurrentUserId();
            if (!await _postService.CheckPostExist(model.PostId))
                throw new NotFoundPostGramException("Post not found: " + model.PostId);

            Guid commentId = await _postService.CreateComment(model, userId);
            return commentId;
        }

        [HttpGet]
        public async Task<CommentModel> GetComment(Guid commentId)
        {
            CommentModel model = await _postService.GetComment(commentId);
            return model;
        }

        [HttpGet]
        public async Task<CommentModel[]> GetCommentsForPost(Guid postId)
        {
            CommentModel[] model = await _postService.GetCommentsForPost(postId);
            return model;
        }

        [HttpDelete]
        public async Task<Guid> DeleteComment(Guid commentId)
        {
            return await _postService.DeleteComment(commentId);
        }

        [HttpPut]
        public async Task<CommentModel> UpdateComment(UpdateCommentModel model)
        {
            return await _postService.UpdateComment(model);
        }

        [HttpPost]
        public async Task<Guid> CreateLike(CreateLikeModel model)
        {
            return await _postService.CreateLike(model, this.GetCurrentUserId());
        }
        //TODO 2 разделить на несколько апишек
        //TODO 2 Сделать подписки пользователей
        //TODO 2 Сделать лайки постов
        //TODO 2 Сделать лайки коментов
    }
}
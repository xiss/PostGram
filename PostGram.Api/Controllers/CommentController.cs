﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostGram.Api.Helpers;
using PostGram.Api.Models.Comment;
using PostGram.Api.Services;
using PostGram.Common.Exceptions;
using LogLevel = NLog.LogLevel;

namespace PostGram.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]

    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly NLog.Logger _logger;
        private readonly IPostService _postService;

        public CommentController(ICommentService commentService, IPostService postService)
        {
            _commentService = commentService;
            _logger = NLog.LogManager.GetCurrentClassLogger();
            _postService = postService;
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateComment(CreateCommentModel model)
        {
            Guid userId = this.GetCurrentUserId();

            try
            {
                if (!await _postService.CheckPostExist(model.PostId))
                    throw new NotFoundPostGramException("Post not found: " + model.PostId);

                Guid commentId = await _commentService.CreateComment(model, userId);
                return Ok(commentId);
            }
            catch (CriticalPostGramException e)
            {
                _logger.Log(LogLevel.Error, e);
                return StatusCode(500, e.Message);
            }
            catch (NotFoundPostGramException e)
            {
                _logger.Log(LogLevel.Warn, e);
                return NotFound(e.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<CommentModel>> GetComment(Guid commentId)
        {
            try
            {
                CommentModel model = await _commentService.GetComment(commentId);
                return Ok(model);
            }
            catch (NotFoundPostGramException e)
            {
                _logger.Log(LogLevel.Warn, e);
                return NotFound(e.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<CommentModel[]>> GetCommentsForPost(Guid postId)
        {
            try
            {
                CommentModel[] model = await _commentService.GetCommentsForPost(postId);
                return Ok(model);
            }
            catch (NotFoundPostGramException e)
            {
                _logger.Log(LogLevel.Warn, e);
                return NotFound(e.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> DeleteComment(Guid commentId)
        {
            try
            {
                return await _commentService.DeleteComment(commentId);
            }
            catch (CriticalPostGramException e)
            {
                _logger.Log(LogLevel.Error, e);
                return StatusCode(500, e.Message);
            }
            catch (NotFoundPostGramException e)
            {
                _logger.Log(LogLevel.Error, e);
                return NotFound(e.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<CommentModel>> UpdateComment(UpdateCommentModel model)
        {
            try
            {
                return await _commentService.UpdateComment(model);
            }
            catch (CriticalPostGramException e)
            {
                _logger.Log(LogLevel.Error, e);
                return StatusCode(500, e.Message);
            }
            catch (NotFoundPostGramException e)
            {
                _logger.Log(LogLevel.Error, e);
                return NotFound(e.Message);
            }
        }
    }
}
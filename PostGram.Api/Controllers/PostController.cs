using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostGram.Api.Models;
using PostGram.Api.Services;
using PostGram.Common.Exceptions;
using LogLevel = NLog.LogLevel;

namespace PostGram.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly NLog.Logger _logger;

        public PostController(IPostService postService)
        {
            _postService = postService;
            _logger = NLog.LogManager.GetCurrentClassLogger();
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Guid>> CreatePost(CreatePostModel model)
        {
            try
            {
                Guid postId = await _postService.CreatePost(model, this.GetCurrentUserId());
                return Ok(postId);
            }
            catch (DBCreatePostGramException e)
            {
                _logger.Log(LogLevel.Error, e);
                return StatusCode(500, e.Message);
            }
            catch (AuthorizationPostGramException e)
            {
                _logger.Log(LogLevel.Warn, e);
                return Forbid(e.Message);
            }
            catch (DBPostGramException e)
            {
                _logger.Log(LogLevel.Error, e);
                return Forbid(e.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<PostModel>> GetPost(Guid postId)
        {
            try
            {
                PostModel postModel = await _postService.GetPost(postId);
                for (int i = 0; i < postModel.Attachments.Length; i++)
                {
                    postModel.Attachments[i] = Url.Action(
                        nameof(AttachmentController.GetAttachment),
                        "Attachment",
                        new { attahmentId = postModel.Attachments[i] },
                        null)!;
                }
                return Ok(postModel);
            }
            catch (PostPostGramException e)
            {
                _logger.Log(LogLevel.Error, e);
                return Forbid(e.Message);
            }
        }

        //TODO 2 DeletePost
        //TODO 2 UpdatePost
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostGram.Api.Helpers;
using PostGram.Api.Models.Attachment;
using PostGram.Api.Models.Post;
using PostGram.Api.Services;
using PostGram.Common.Exceptions;
using LogLevel = NLog.LogLevel;

namespace PostGram.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    //[Authorize]
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
        public async Task<ActionResult<Guid>> CreatePost(CreatePostModel model)
        {
            try
            {
                Guid postId = await _postService.CreatePost(model, this.GetCurrentUserId());
                return Ok(postId);
            }
            catch (DbPostGramException e)
            {
                _logger.Log(LogLevel.Error, e);
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<PostModel>> GetPost(Guid postId)
        {
            try
            {
                PostModel model = await _postService.GetPost(postId);
                model.Author.Avatar.Link = AttachmentController.GetLinkForAvatar(Url, model.Author.Id);
                foreach (AttachmentModel attachment in model.Content)
                {
                    attachment.Link = AttachmentController.GetLinkForPostContent(Url, attachment.Id);
                }

                return Ok(model);
            }
            catch (CriticalPostGramException e)
            {
                _logger.Log(LogLevel.Error, e);
                return StatusCode(500, e.Message);
            }
            catch (CommonPostGramException e)
            {
                _logger.Log(LogLevel.Error, e);
                return StatusCode(500, e.Message);
            }
        }

        //TODO 2 DeletePost
        //TODO 2 UpdatePost
        //TODO 2 GetPosts
    }
}
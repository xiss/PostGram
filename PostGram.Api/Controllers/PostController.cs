using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostGram.BLL.Interfaces.Base.Commands;
using PostGram.BLL.Interfaces.Base.Queries;
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
    private readonly ICommandHandler<CreatePostCommand> _createPostHandler;
    private readonly ICommandHandler<DeletePostCommand> _deletePostHandler;
    private readonly IQueryHandler<GetPostQuery, GetPostResult> _getPostHandler;
    private readonly IQueryHandler<GetPostsQuery, GetPostsResult> _getPostsHandler;
    private readonly ICommandHandler<UpdatePostCommand> _updatePostHandler;

    public PostController(
        ICommandHandler<CreatePostCommand> createPostHandler,
        ICommandHandler<DeletePostCommand> deletePostHandler,
        IQueryHandler<GetPostQuery, GetPostResult> getPostHandler,
        IQueryHandler<GetPostsQuery, GetPostsResult> getPostsHandler,
        ICommandHandler<UpdatePostCommand> updatePostHandler)
    {
        _createPostHandler = createPostHandler ?? throw new ArgumentNullException(nameof(createPostHandler));
        _deletePostHandler = deletePostHandler ?? throw new ArgumentNullException(nameof(deletePostHandler));
        _getPostHandler = getPostHandler ?? throw new ArgumentNullException(nameof(getPostHandler));
        _getPostsHandler = getPostsHandler ?? throw new ArgumentNullException(nameof(getPostsHandler));
        _updatePostHandler = updatePostHandler ?? throw new ArgumentNullException(nameof(updatePostHandler));
    }

    [HttpPost]
    public async Task CreatePost(CreatePostCommand command)
    {
        await _createPostHandler.Execute(command);
    }

    [HttpDelete]
    public async Task DeletePost(DeletePostCommand command)
    {
        await _deletePostHandler.Execute(command);
    }

    [HttpGet]
    public async Task<GetPostResult> GetPost(GetPostQuery query)
    {
        return await _getPostHandler.Execute(query);
    }

    [HttpGet]
    public async Task<GetPostsResult> GetPosts(GetPostsQuery query)
    {
        return await _getPostsHandler.Execute(query);
    }

    [HttpPut]
    public async Task UpdatePost(UpdatePostCommand command)
    {
        await _updatePostHandler.Execute(command);
    }

    //TODO ужимать картинки и делать 2 версии разного размера
}
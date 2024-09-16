using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PostGram.BLL.Interfaces.Providers;
using PostGram.BLL.Interfaces.Services;
using PostGram.Common.Exceptions;
using PostGram.Common.Interfaces.Base.Commands;
using PostGram.Common.Requests.Commands;
using PostGram.DAL;
using PostGram.DAL.Entities;

namespace PostGram.BLL.Handlers.Commands;

public class CreateCommentHandler : ICommandHandler<CreateCommentCommand>
{
    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;
    private readonly IClaimsProvider _claimsProvider;
    private readonly ICommentService _commentService;

    public CreateCommentHandler(DataContext dataContext, IMapper mapper, IClaimsProvider claimsProvider, ICommentService commentService)
    {
        _dataContext = dataContext ;
        _mapper = mapper;
        _claimsProvider = claimsProvider;
        _commentService = commentService;
    }

    public async Task Execute(CreateCommentCommand command)
    {
        if (!await CheckPostExist(command.PostId))
            throw new NotFoundPostGramException("Post not found: " + command.PostId);
        if (command.QuotedCommentId == null ^ command.QuotedText == null)
        {
            throw new UnprocessableRequestPostGramException(
                "QuotedCommentId and QuotedText must be null at the same time or must have value at the same time");
        }

        Comment comment = _mapper.Map<Comment>(command);
        comment.AuthorId = _claimsProvider.GetCurrentUserId();

        if (command.QuotedCommentId != null && command.QuotedText != null)
        {
            Comment quotedComment = await _commentService.GetCommentById(command.QuotedCommentId.Value);
            comment.QuotedCommentId = quotedComment.Id;

            if (!quotedComment.Body.Contains(command.QuotedText))
                throw new UnprocessableRequestPostGramException(
                    $"Quoted comment {command.QuotedCommentId} does not contain quoted text");
            comment.QuotedText = command.QuotedText;
        }

        await _dataContext.Comments.AddAsync(comment);
        await _dataContext.SaveChangesAsync();
    }

    private async Task<bool> CheckPostExist(Guid postId)
    {
        return await _dataContext.Posts.AnyAsync(u => u.Id == postId);
    }
}
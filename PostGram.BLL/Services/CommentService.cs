using Microsoft.EntityFrameworkCore;
using PostGram.BLL.Interfaces.Services;
using PostGram.Common.Exceptions;
using PostGram.DAL;
using PostGram.DAL.Entities;

namespace PostGram.BLL.Services;

public class CommentService : ICommentService
{
    private readonly DataContext _dataContext;

    public CommentService(DataContext dataContext)
    {
        _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
    }

    public async Task<Comment> GetCommentById(Guid commentId)
    {
        Comment comment = await _dataContext.Comments
                .Include(p => p.Likes)
                .Include(c => c.Author)
                .ThenInclude(a => a.Avatar)
                .FirstOrDefaultAsync(c => c.Id == commentId)
            ?? throw new NotFoundPostGramException("Comment not found: " + commentId);
        return comment;
    }

    public async Task UpdateComment(Comment comment)
    {
        _dataContext.Comments.Update(comment);
        await _dataContext.SaveChangesAsync();
    }
}
using PostGram.DAL.Entities;

namespace PostGram.BLL.Interfaces.Services;

public interface ICommentService
{
    Task<Comment> GetCommentById(Guid commentId);

    Task UpdateComment(Comment comment);
}
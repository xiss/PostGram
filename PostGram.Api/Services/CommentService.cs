using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PostGram.Api.Models.Comment;
using PostGram.Common.Exceptions;
using PostGram.DAL;
using PostGram.DAL.Entities;

namespace PostGram.Api.Services
{
    public class CommentService : ICommentService
    {
        private readonly DataContext _dataContext;
        private readonly NLog.Logger _logger;
        private readonly IMapper _mapper;

        public CommentService(DataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _logger = NLog.LogManager.GetCurrentClassLogger();
            _mapper = mapper;
        }

        public async Task<Guid> CreateComment(CreateCommentModel model, Guid userId)
        {
            Comment comment = _mapper.Map<Comment>(model);
            comment.AuthorId = userId;

            try
            {
                await _dataContext.Comments.AddAsync(comment);
                await _dataContext.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException != null)
                {
                    throw new DbPostGramException(e.InnerException.Message, e.InnerException);
                }
                throw new DbPostGramException(e.Message, e);
            }

            return comment.Id;
        }

        private async Task<Comment> GetCommentById(Guid commentId)
        {
            Comment? comment = await _dataContext.Comments
                .FirstOrDefaultAsync(c => c.Id == commentId && !c.IsDeleted);
            if (comment == null)
                throw new NotFoundPostGramException("Comment not found: " + commentId);
            return comment;
        }

        public async Task<CommentModel> GetComment(Guid commentId)
        {
            Comment comment = await GetCommentById(commentId);
            return _mapper.Map<CommentModel>(comment);
        }

        public async Task<CommentModel[]> GetCommentsForPost(Guid postId)
        {
            Comment[]? comments = await _dataContext.Comments
                .AsNoTracking()
                .Where(c => c.PostId == postId && !c.IsDeleted && !c.Post.IsDeleted)
                .OrderBy(c => c.Created)
                .ToArrayAsync();
            if (comments.Length == 0)
                throw new NotFoundPostGramException("Comments not fount for post: " + postId);

            return _mapper.Map<CommentModel[]>(comments);
        }

        public async Task<bool> CheckCommentExist(Guid commentId)
        {
            return await _dataContext.Comments.AnyAsync(u => u.Id == commentId);
        }

        public async Task<Guid> DeleteComment(Guid commentId)
        {
            Comment comment = await GetCommentById(commentId);
            comment.IsDeleted = true;
            await UpdateComment(comment);

            return commentId;
        }

        public async Task<CommentModel> UpdateComment(UpdateCommentModel model)
        {
            Comment comment = await GetCommentById(model.Id);
            comment.Body = model.NewBody;
            comment.Edited = DateTimeOffset.UtcNow;
            await UpdateComment(comment);
            return _mapper.Map<CommentModel>(comment);
        }

        private async Task UpdateComment(Comment comment)
        {
            try
            {
                _dataContext.Comments.Update(comment);
                await _dataContext.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException != null)
                {
                    throw new DbPostGramException(e.InnerException.Message, e.InnerException);
                }
                throw new DbPostGramException(e.Message, e);
            }
        }
    }
}
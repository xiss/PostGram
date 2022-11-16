using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PostGram.Api.Models.Attachment;
using PostGram.Api.Models.Comment;
using PostGram.Api.Models.Like;
using PostGram.Api.Models.Post;
using PostGram.Common.Exceptions;
using PostGram.DAL;
using PostGram.DAL.Entities;

namespace PostGram.Api.Services
{
    public class PostService : IPostService
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        private readonly IAttachmentService _attachmentService;

        public PostService(DataContext dataContext, IAttachmentService attachmentService, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
            _attachmentService = attachmentService;
        }

        public async Task<Guid> CreatePost(CreatePostModel model, Guid userId)
        {
            Post post = _mapper.Map<Post>(model);
            post.AuthorId = userId;

            try
            {
                foreach (var metadataModel in model.Attachments)
                {
                    PostContent postContent = _mapper.Map<PostContent>(metadataModel);
                    postContent.AuthorId = userId;
                    postContent.FilePath = await _attachmentService.ApplyFile(metadataModel.TempId.ToString());
                    post.PostContents.Add(postContent);
                }

                await _dataContext.Posts.AddAsync(post);
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

            return post.Id;
        }

        public async Task<bool> CheckPostExist(Guid postId)
        {
            return await _dataContext.Posts.AnyAsync(u => u.Id == postId);
        }

        public async Task<PostModel> GetPost(Guid postId)
        {
            Post? post = await _dataContext.Posts
                .AsNoTracking()
                .Include(p => p.PostContents)
                .Include(p => p.Author)
                .ThenInclude(a => a.Avatar)
                .Include(p => p.Comments
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.Created))
                .FirstOrDefaultAsync(p => p.Id == postId && !p.IsDeleted);
            if (post == null)
                throw new NotFoundPostGramException("Post not found: " + postId);

            return _mapper.Map<PostModel>(post);
        }

        public async Task<List<PostModel>> GetPosts(int take, int skip)
        {
            List<PostModel> model = await _dataContext.Posts
                .AsNoTracking()
                .Include(p => p.PostContents)
                .Include(p => p.Comments)
                .Include(p => p.Author)
                .ThenInclude(a => a.Avatar)
                .OrderByDescending(p => p.Created)
                .Where(p => p.IsDeleted != true)
                .Skip(skip)
                .Take(take)
                .Select(p => _mapper.Map<PostModel>(p))
                .ToListAsync();
            if (model.Count == 0)
                throw new NotFoundPostGramException("Posts not found");

            return model;
        }

        public async Task DeletePost(Guid postId, Guid userId)
        {
            Post? post = await _dataContext.Posts
                .Include(p => p.PostContents)
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null)
                throw new NotFoundPostGramException($"Post {postId} not found");

            if (post.AuthorId != userId)
                throw new AuthorizationPostGramException("Cannot delete post created by another user id");

            post.IsDeleted = true;

            if (post.PostContents != null)
                await DeletePostContents(post.PostContents);

            if (post.Comments != null)
            {
                foreach (Comment comment in post.Comments)
                    await DeleteComment(comment.Id);
            }

            try
            {
                _dataContext.Posts.Update(post);
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

        public async Task<PostModel> UpdatePost(UpdatePostModel model, Guid curentUserId)
        {
            Post? post = await _dataContext.Posts
                .Include(p => p.PostContents)
                .Include(p => p.Author)
                .ThenInclude(a => a.Avatar)
                .Include(p => p.Comments
                    .Where(c => !c.IsDeleted)
                .OrderBy(c => c.Created))
                .FirstOrDefaultAsync(u => u.Id == model.Id && !u.IsDeleted);
            if (post == null)
                throw new NotFoundPostGramException("Post not found: " + model.Id);

            if (post.AuthorId != curentUserId)
                throw new AuthorizationPostGramException("Cannot modify post created by another user");
            //Body
            if (model.UpdatedBody != null)
                post.Body = model.UpdatedBody;
            //Header
            if (model.UpdatedHeader != null)
                post.Header = model.UpdatedHeader;
            //Add new content
            if (model.NewContent.Count > 0)
            {
                foreach (MetadataModel postContent in model.NewContent)
                {
                    //Если это повторный запрос и мы уже добавили, второй раз не добавляем.
                    if (post.PostContents.Any(pc => pc.Id == postContent.TempId))
                        continue;

                    _dataContext.PostContents.Add(new PostContent()
                    {
                        Id = postContent.TempId,
                        PostId = post.Id,
                        AuthorId = curentUserId,
                        Created = DateTimeOffset.UtcNow,
                        FilePath = await _attachmentService.ApplyFile(postContent.TempId.ToString()),
                        MimeType = postContent.MimeType,
                        Name = postContent.Name,
                        Size = postContent.Size,
                    });
                }
            }
            //Edit timestamp
            post.Edited = DateTimeOffset.UtcNow;
            //Delete postContent
            if (model.ContentToDelete.Count > 0)
            {
                List<PostContent> postContentsToDelete =
                    await GetPostContents(model.ContentToDelete.Select(am => am.Id).ToList());
                await DeletePostContents(postContentsToDelete);
            }
            //Save
            try
            {
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

            return _mapper.Map<PostModel>(post);
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
                throw new NotFoundPostGramException("Comments not found for post: " + postId);

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

        //TODO 1 To test
        public async Task<Guid> CreateLike(CreateLikeModel model, Guid curentUserId)
        {
            if (model.CommentId == null && model.PostId == null)
                throw new BadRequestPostGramException("Bad model, Comment and Post cannot be null both");
            if (model.PostId != null && model.CommentId != null)
                throw new BadRequestPostGramException("Bad model, Comment and Post cannot be not null both");

            Like like = _mapper.Map<Like>(model);
            like.AuthorId = curentUserId;

            //Post
            if (model.PostId != null && await CheckPostExist(model.PostId.Value))
                like.PostId = model.PostId;
            else
                throw new NotFoundPostGramException($"Post {model.PostId} not found");
            //Comment
            if (model.CommentId != null && await CheckCommentExist(model.CommentId.Value))
                like.CommentId = model.CommentId;
            else
                throw new NotFoundPostGramException($"Comment {model.CommentId} not found");

            try
            {
                _dataContext.Likes.Add(like);
                await _dataContext.SaveChangesAsync();
                return like.Id;
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

        private async Task DeletePostContents(ICollection<PostContent> postContents)
        {

            foreach (var item in postContents)
            {
                _attachmentService.DeleteFile(item.Id);
            }
            try
            {
                _dataContext.PostContents.RemoveRange(postContents);
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

        private async Task<List<PostContent>> GetPostContents(List<Guid> postContentIds)
        {
            if (postContentIds.Count == 0 || postContentIds == null)
                throw new ArgumentPostGramException("Input colection is empty or null");

            List<PostContent> postContent = await _dataContext.PostContents
                .Where(pc => postContentIds
                .Contains(pc.Id)).ToListAsync();

            if (postContent == null)
                throw new NotFoundPostGramException($"Post contents {string.Join(' ', postContentIds)} not found in DB");
            return postContent;
        }

        private async Task<Comment> GetCommentById(Guid commentId)
        {
            Comment? comment = await _dataContext.Comments
                .FirstOrDefaultAsync(c => c.Id == commentId && !c.IsDeleted);
            if (comment == null)
                throw new NotFoundPostGramException("Comment not found: " + commentId);
            return comment;
        }
    }
}
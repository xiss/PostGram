using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PostGram.Api.Models.Attachment;
using PostGram.Api.Models.Comment;
using PostGram.Api.Models.Like;
using PostGram.Api.Models.Post;
using PostGram.Common.Enums;
using PostGram.Common.Exceptions;
using PostGram.DAL;
using PostGram.DAL.Entities;

namespace PostGram.Api.Services
{
    public class PostService : IDisposable, IPostService
    {
        private readonly IAttachmentService _attachmentService;
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        public PostService(DataContext dataContext, IAttachmentService attachmentService, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
            _attachmentService = attachmentService;
        }

        public async Task<Guid> CreateComment(CreateCommentModel model, Guid currentUserId)
        {
            if (!await CheckPostExist(model.PostId))
                throw new NotFoundPostGramException("Post not found: " + model.PostId);
            if (model.QuotedCommentId == null ^ model.QuotedText == null)
            {
                throw new UnprocessableRequestPostGramException(
                    "QuotedCommentId and QuotedText must be null at the same time or must have value at the same time");
            }

            Comment comment = _mapper.Map<Comment>(model);
            comment.AuthorId = currentUserId;

            if (model.QuotedCommentId != null && model.QuotedText != null)
            {
                Comment quotedComment = await GetCommentById(model.QuotedCommentId.Value);
                comment.QuotedCommentId = quotedComment.Id;

                if (!quotedComment.Body.Contains(model.QuotedText))
                    throw new UnprocessableRequestPostGramException(
                        $"Quoted comment {model.QuotedCommentId} does not contain quoted text");
                comment.QuotedText = model.QuotedText;
            }
            try
            {
                await _dataContext.Comments.AddAsync(comment);
                await _dataContext.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException != null)
                {
                    throw new PostGramException(e.InnerException.Message, e.InnerException);
                }
                throw new PostGramException(e.Message, e);
            }

            return comment.Id;
        }

        public async Task<Guid> CreateLike(CreateLikeModel model, Guid currentUserId)
        {
            Like like = _mapper.Map<Like>(model);
            like.AuthorId = currentUserId;

            if (await CheckLikeExist(currentUserId, like.EntityId))
                throw new UnprocessableRequestPostGramException(
                    $"User {like.AuthorId} already has like for entity '{like.EntityType}' - {like.EntityId}");

            switch (like.EntityType)
            {
                case LikableEntities.Post:
                    Post? post = await _dataContext.Posts.FirstOrDefaultAsync(p => p.Id == like.EntityId);
                    if (post == null)
                        throw new NotFoundPostGramException($"Post {like.EntityId} not found");
                    post.Likes.Add(like);
                    break;

                case LikableEntities.Comment:
                    Comment? comment = await _dataContext.Comments.FirstOrDefaultAsync(c => c.Id == like.EntityId);
                    if (comment == null)
                        throw new NotFoundPostGramException($"Comment {like.EntityId} not found");
                    comment.Likes.Add(like);
                    break;

                default:
                    throw new PostGramException("Unregistered entity type");
            }

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
                    throw new PostGramException(e.InnerException.Message, e.InnerException);
                }
                throw new PostGramException(e.Message, e);
            }
        }

        public async Task<Guid> CreatePost(CreatePostModel model, Guid currentUserId)
        {
            Post post = _mapper.Map<Post>(model);
            post.AuthorId = currentUserId;

            try
            {
                foreach (var metadataModel in model.Attachments)
                {
                    PostContent postContent = _mapper.Map<PostContent>(metadataModel);
                    postContent.AuthorId = currentUserId;
                    await _attachmentService.ApplyFile(metadataModel.TempId.ToString());
                    post.PostContents.Add(postContent);
                }

                await _dataContext.Posts.AddAsync(post);
                await _dataContext.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException != null)
                {
                    throw new PostGramException(e.InnerException.Message, e.InnerException);
                }
                throw new PostGramException(e.Message, e);
            }

            return post.Id;
        }

        public async Task<Guid> DeleteComment(Guid commentId, Guid currentUserId)
        {
            Comment comment = await GetCommentById(commentId);
            if (comment.AuthorId != currentUserId)
                throw new AuthorizationPostGramException("Cannot delete comment created by another user");
            comment.IsDeleted = true;
            await UpdateComment(comment);

            return commentId;
        }

        public async Task<Guid> DeletePost(Guid postId, Guid currentUserId)
        {
            Post? post = await _dataContext.Posts
                .Include(p => p.PostContents)
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null)
                throw new NotFoundPostGramException($"Post {postId} not found");

            if (post.AuthorId != currentUserId)
                throw new AuthorizationPostGramException("Cannot delete post created by another user id");

            post.IsDeleted = true;

            await DeletePostContents(post.PostContents);

            try
            {
                _dataContext.Posts.Update(post);
                await _dataContext.SaveChangesAsync();
                return postId;
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException != null)
                {
                    throw new PostGramException(e.InnerException.Message, e.InnerException);
                }
                throw new PostGramException(e.Message, e);
            }
        }

        public void Dispose()
        {
            _dataContext.Dispose();
            _attachmentService.Dispose();
        }

        public async Task<CommentModel> GetComment(Guid commentId, Guid currentUserId)
        {
            Comment comment = await GetCommentById(commentId);
            CommentModel result = _mapper.Map<CommentModel>(comment);

            result.LikeByUser = _mapper
                .Map<LikeModel>(comment.Likes
                    .FirstOrDefault(l => l.AuthorId == currentUserId));
            return result;
        }

        public async Task<CommentModel[]> GetCommentsForPost(Guid postId, Guid currentUserId)
        {
            Comment[] comments = await _dataContext.Comments
                .Include(c => c.Likes)
                .Include(c => c.Author)
                .ThenInclude(a => a.Avatar)
                .AsNoTracking()
                .Where(c => c.PostId == postId)
                .OrderBy(c => c.Created)
                .ToArrayAsync();
            if (comments.Length == 0)
                throw new NotFoundPostGramException("Comments not found for post: " + postId);

            CommentModel[] result = _mapper.Map<CommentModel[]>(comments);
            foreach (var comment in result)
            {
                comment.LikeByUser = _mapper
                    .Map<LikeModel>(comments
                        .FirstOrDefault(c => c.Id == comment.Id)?.Likes
                        .FirstOrDefault(l => l.AuthorId == currentUserId));
            }
            return result;
        }

        public async Task<PostModel> GetPost(Guid postId, Guid currentUserId)
        {
            List<Guid> subscriptions = await GetAvailableSubscriptionsForSlaveUser(currentUserId);

            Post? post = await _dataContext.Posts
                .AsNoTracking()
                .Include(p => p.PostContents)
                .Include(p => p.Likes)
                .Include(p => p.Author)
                .ThenInclude(a => a.Avatar)
                .Include(p => p.Comments
                    .OrderBy(c => c.Created))
                .ThenInclude(c => c.Likes)
                .FirstOrDefaultAsync(p => p.Id == postId);
            if (post == null)
                throw new NotFoundPostGramException("Post not found: " + postId);

            if (post.AuthorId != currentUserId && !subscriptions.Contains(post.AuthorId))
                throw new AuthorizationPostGramException($"User {currentUserId} cannot access post {post.Id}");

            PostModel result = _mapper.Map<PostModel>(post);

            result.LikeByUser = _mapper
                .Map<LikeModel>(post.Likes
                    .FirstOrDefault(l => l.AuthorId == currentUserId));
            return result;
        }

        public async Task<List<PostModel>> GetPosts(int takeAmount, int skipAmount, Guid currentUserId)
        {
            List<Guid> subscriptions = await GetAvailableSubscriptionsForSlaveUser(currentUserId);

            List<Post> posts = await _dataContext.Posts
                .Include(p => p.PostContents)
                .Include(p => p.Author)
                .ThenInclude(u => u.Slaves)
                .Include(p => p.Comments)
                .ThenInclude(c => c.Likes)
                .Include(p => p.Likes)
                .Include(p => p.Author)
                .ThenInclude(a => a.Avatar)
                .Where(p => subscriptions
                    .Contains(p.AuthorId) || p.AuthorId == currentUserId)
                .OrderByDescending(p => p.Created)
                .Skip(skipAmount)
                .Take(takeAmount)
                .AsNoTracking()
                .ToListAsync();

            if (posts.Count == 0)
                throw new NotFoundPostGramException("Posts not found");

            List<PostModel> result = posts.Select(p => _mapper.Map<PostModel>(p)).ToList();
            foreach (var post in result)
            {
                post.LikeByUser = _mapper
                    .Map<LikeModel>(posts
                        .FirstOrDefault(p => p.Id == post.Id)?.Likes
                    .FirstOrDefault(l => l.AuthorId == currentUserId));
            }
            return result;
        }

        public async Task<CommentModel> UpdateComment(UpdateCommentModel model, Guid currentUserId)
        {
            Comment comment = await GetCommentById(model.Id);
            if (comment.AuthorId != currentUserId)
                throw new AuthorizationPostGramException("Cannot modify comment created by another user");
            comment.Body = model.NewBody;
            comment.Edited = DateTimeOffset.UtcNow;
            await UpdateComment(comment);

            CommentModel result = _mapper.Map<CommentModel>(comment);
            result.LikeByUser = _mapper
                .Map<LikeModel>(comment.Likes
                    .FirstOrDefault(l => l.AuthorId == currentUserId));
            return result;
        }

        public async Task<LikeModel> UpdateLike(UpdateLikeModel model, Guid currentUserId)
        {
            Like? like = await _dataContext.Likes.FirstOrDefaultAsync(l => l.Id == model.Id);
            if (like == null)
                throw new NotFoundPostGramException($"Like {model.Id} not found in DB");

            if (like.AuthorId != currentUserId)
                throw new AuthorizationPostGramException("Cannot modify like created by another user");

            like.IsLike = model.IsLike;
            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException != null)
                {
                    throw new PostGramException(e.InnerException.Message, e.InnerException);
                }
                throw new PostGramException(e.Message, e);
            }

            return _mapper.Map<LikeModel>(like);
        }

        public async Task<PostModel> UpdatePost(UpdatePostModel model, Guid currentUserId)
        {
            Post? post = await _dataContext.Posts
                .Include(p => p.PostContents)
                .Include(p => p.Likes)
                .Include(p => p.Author)
                .ThenInclude(a => a.Avatar)
                .Include(p => p.Comments
                    .OrderBy(c => c.Created))
                .FirstOrDefaultAsync(u => u.Id == model.Id);
            if (post == null)
                throw new NotFoundPostGramException("Post not found: " + model.Id);

            if (post.AuthorId != currentUserId)
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
                        AuthorId = currentUserId,
                        Created = DateTimeOffset.UtcNow,
                        MimeType = postContent.MimeType,
                        Name = postContent.Name,
                        Size = postContent.Size,
                    });
                    await _attachmentService.ApplyFile(postContent.TempId.ToString());
                }
            }
            //Edit timestamp
            post.Edited = DateTimeOffset.UtcNow;
            //Delete postContent
            if (model.ContentToDelete.Count > 0)
            {
                List<PostContent> postContentsToDelete =
                    await GetPostContentsByIds(model.ContentToDelete.Select(am => am.Id).ToList());
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
                    throw new PostGramException(e.InnerException.Message, e.InnerException);
                }
                throw new PostGramException(e.Message, e);
            }

            PostModel result = _mapper.Map<PostModel>(post);
            result.LikeByUser = _mapper
                .Map<LikeModel>(post.Likes
                    .FirstOrDefault(l => l.AuthorId == currentUserId));
            return result;
        }

        private async Task<bool> CheckCommentExist(Guid commentId)
        {
            return await _dataContext.Comments.AnyAsync(u => u.Id == commentId);
        }

        private async Task<bool> CheckLikeExist(Guid authorId, Guid entityId)
        {
            return await _dataContext.Likes.AnyAsync(l => l.AuthorId == authorId && l.EntityId == entityId);
        }

        private async Task<bool> CheckPostExist(Guid postId)
        {
            return await _dataContext.Posts.AnyAsync(u => u.Id == postId);
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
                    throw new PostGramException(e.InnerException.Message, e.InnerException);
                }
                throw new PostGramException(e.Message, e);
            }
        }

        private async Task<List<Guid>> GetAvailableSubscriptionsForSlaveUser(Guid slaveUserId)
        {
            return await _dataContext.Subscriptions
                .Where(s => s.SlaveId == slaveUserId && (s.Status || !s.Master.IsPrivate))
                .Select(s => s.MasterId)
                .ToListAsync();
        }

        private async Task<Comment> GetCommentById(Guid commentId)
        {
            Comment? comment = await _dataContext.Comments
                .Include(p => p.Likes)
                .Include(c => c.Author)
                .ThenInclude(a => a.Avatar)
                .FirstOrDefaultAsync(c => c.Id == commentId);
            if (comment == null)
                throw new NotFoundPostGramException("Comment not found: " + commentId);
            return comment;
        }

        private async Task<List<PostContent>> GetPostContentsByIds(List<Guid> postContentIds)
        {
            if (postContentIds.Count == 0 || postContentIds == null)
                throw new ArgumentPostGramException("Input collection is empty or null");

            List<PostContent> postContent = await _dataContext.PostContents
                .Where(pc => postContentIds
                .Contains(pc.Id)).ToListAsync();

            if (postContent == null)
                throw new NotFoundPostGramException($"Post contents {string.Join(' ', postContentIds)} not found in DB");
            return postContent;
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
                    throw new PostGramException(e.InnerException.Message, e.InnerException);
                }
                throw new PostGramException(e.Message, e);
            }
        }
    }
}
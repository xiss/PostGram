﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
                    post.Attachments.Add(new Attachment()
                    {
                        AuthorId = userId,
                        Created = DateTimeOffset.UtcNow,
                        FilePath = await _attachmentService.ApplyFile(metadataModel.TempId.ToString()),
                        MimeType = metadataModel.MimeType,
                        Name = metadataModel.Name,
                        Size = metadataModel.Size,
                    });
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
                .Include(p => p.Attachments)
                .Include(p => p.Comments
                    .Where(c => !c.IsDeleted)
                .OrderBy(c => c.Created))
                .FirstOrDefaultAsync(u => u.Id == postId && !u.IsDeleted);
            if (post == null)
                throw new NotFoundPostGramException("Post not found: " + postId);

            PostModel postModel = _mapper.Map<PostModel>(post);
            postModel.Attachments = post.Attachments.Select(a => a.Id.ToString()).ToArray();

            return postModel;
        }
    }
}
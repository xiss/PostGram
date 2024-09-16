using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PostGram.BLL.Interfaces.Providers;
using PostGram.BLL.Interfaces.Services;
using PostGram.Common.Dtos.Attachment;
using PostGram.Common.Exceptions;
using PostGram.Common.Interfaces.Base.Commands;
using PostGram.Common.Requests.Commands;
using PostGram.DAL;
using PostGram.DAL.Entities;

namespace PostGram.BLL.Handlers.Commands;

public class CreatePostHandler : ICommandHandler<CreatePostCommand>
{
    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;
    private readonly IClaimsProvider _claimsProvider;
    private readonly IAttachmentService _attachmentService;

    public CreatePostHandler(DataContext dataContext, IMapper mapper, IClaimsProvider claimsProvider, IAttachmentService attachmentService)
    {
        _dataContext = dataContext ;
        _mapper = mapper;
        _claimsProvider = claimsProvider;
        _attachmentService = attachmentService;
    }

    public async Task Execute(CreatePostCommand command)
    {
        Guid userId = _claimsProvider.GetCurrentUserId();
        Post post = _mapper.Map<Post>(command);
        post.AuthorId = userId;

        try
        {
            foreach (MetadataDto metadataModel in command.Attachments)
            {
                PostContent postContent = _mapper.Map<PostContent>(metadataModel);
                postContent.AuthorId = userId;
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
    }
}
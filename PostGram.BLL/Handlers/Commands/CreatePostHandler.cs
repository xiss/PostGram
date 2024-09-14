using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PostGram.BLL.Interfaces.Base.Commands;
using PostGram.BLL.Interfaces.Providers;
using PostGram.BLL.Interfaces.Services;
using PostGram.Common.Dtos.Attachment;
using PostGram.Common.Exceptions;
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
        _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _claimsProvider = claimsProvider ?? throw new ArgumentNullException(nameof(claimsProvider));
        _attachmentService = attachmentService ?? throw new ArgumentNullException(nameof(attachmentService));
    }

    public async Task Execute(CreatePostCommand command)
    {
        Guid userId = _claimsProvider.GetCurrentUserId();
        Post post = _mapper.Map<Post>(command);
        post.AuthorId = userId;

        try
        {
            foreach (MetadataModel metadataModel in command.Attachments)
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
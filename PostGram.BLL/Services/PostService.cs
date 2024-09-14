using Microsoft.EntityFrameworkCore;
using PostGram.BLL.Interfaces.Services;
using PostGram.Common.Exceptions;
using PostGram.DAL;
using PostGram.DAL.Entities;

namespace PostGram.BLL.Services;

public class PostService : IPostService
{
    private readonly DataContext _dataContext;
    private readonly IAttachmentService _attachmentService;

    public PostService(DataContext dataContext, IAttachmentService attachmentService)
    {
        _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        _attachmentService = attachmentService ?? throw new ArgumentNullException(nameof(attachmentService));
    }

    public async Task DeletePostContents(ICollection<PostContent> postContents)
    {
        foreach (PostContent item in postContents)
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
}
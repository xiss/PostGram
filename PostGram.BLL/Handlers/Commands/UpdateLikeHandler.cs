using Microsoft.EntityFrameworkCore;
using PostGram.BLL.Interfaces.Base.Commands;
using PostGram.BLL.Interfaces.Providers;
using PostGram.Common.Exceptions;
using PostGram.Common.Requests.Commands;
using PostGram.DAL;
using PostGram.DAL.Entities;

namespace PostGram.BLL.Handlers.Commands;

public class UpdateLikeHandler : ICommandHandler<UpdateLikeCommand>
{
    private readonly DataContext _dataContext;
    private readonly IClaimsProvider _claimsProvider;

    public UpdateLikeHandler(DataContext dataContext, IClaimsProvider claimsProvider)
    {
        _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        _claimsProvider = claimsProvider ?? throw new ArgumentNullException(nameof(claimsProvider));
    }

    public async Task Execute(UpdateLikeCommand command)
    {
        Guid userId = _claimsProvider.GetCurrentUserId();
        Like like = await _dataContext.Likes.FirstOrDefaultAsync(l => l.Id == command.Id)
            ?? throw new NotFoundPostGramException($"Like {command.Id} not found in DB");
        if (like.AuthorId != userId)
            throw new AuthorizationPostGramException("Cannot modify like created by another user");

        like.IsLike = command.IsLike;
        await _dataContext.SaveChangesAsync();
    }
}
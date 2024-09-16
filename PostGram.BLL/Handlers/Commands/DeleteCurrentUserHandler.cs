using Microsoft.EntityFrameworkCore;
using PostGram.BLL.Interfaces.Providers;
using PostGram.BLL.Interfaces.Services;
using PostGram.Common.Exceptions;
using PostGram.Common.Interfaces.Base.Commands;
using PostGram.Common.Requests.Commands;
using PostGram.DAL;
using PostGram.DAL.Entities;

namespace PostGram.BLL.Handlers.Commands;

public class DeleteCurrentUserHandler : ICommandHandler<DeleteCurrentUserCommand>
{
    private readonly DataContext _dataContext;
    private readonly IClaimsProvider _claimsProvider;
    private readonly IUserService _userService;

    public DeleteCurrentUserHandler(DataContext dataContext, IClaimsProvider claimsProvider, IUserService userService)
    {
        _dataContext = dataContext ;
        _claimsProvider = claimsProvider;
        _userService = userService;
    }

    public async Task Execute(DeleteCurrentUserCommand command)
    {
        Guid userId = _claimsProvider.GetCurrentUserId();

        User user = await _userService.GetUserById(userId);
        if (user == null)
            throw new NotFoundPostGramException($"User {userId} not found");
        user.IsDelete = true;

        await _dataContext.UserSessions.Where(us => us.UserId == userId).ForEachAsync(us => us.IsActive = false);
        await _dataContext.SaveChangesAsync();
    }
}
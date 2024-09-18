using Microsoft.EntityFrameworkCore;
using PostGram.BLL.Interfaces.Providers;
using PostGram.Common.Exceptions;
using PostGram.Common.Interfaces.Base.Commands;
using PostGram.Common.Requests.Commands;
using PostGram.DAL;
using PostGram.DAL.Entities;

namespace PostGram.BLL.Handlers.Commands;

public class LogoutHandler : ICommandHandler<LogoutCommand>
{
    private readonly IClaimsProvider _claimsProvider;
    private readonly DataContext _dataContext;


    public LogoutHandler(IClaimsProvider claimsProvider, DataContext dataContext)
    {
        _claimsProvider = claimsProvider;
        _dataContext = dataContext;
    }

    public async Task Execute(LogoutCommand command)
    {
        Guid userId = _claimsProvider.GetCurrentUserId();
        Guid sessionId = _claimsProvider.GetCurrentSessionId();

        UserSession? session = await _dataContext.UserSessions
            .FirstOrDefaultAsync(us => us.Id == sessionId);
        if (session == null)
            throw new NotFoundPostGramException($"Session: {sessionId} for user: {userId} not found");
        session.IsActive = false;

        _dataContext.UserSessions.Update(session);
        await _dataContext.SaveChangesAsync();

    }
}
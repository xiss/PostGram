using Microsoft.EntityFrameworkCore;
using PostGram.BLL.Interfaces.Providers;
using PostGram.Common.Exceptions;
using PostGram.Common.Interfaces.Base.Commands;
using PostGram.Common.Requests.Commands;
using PostGram.DAL;
using PostGram.DAL.Entities;

namespace PostGram.BLL.Handlers.Commands;

public class UpdateSubscriptionHandler : ICommandHandler<UpdateSubscriptionCommand>
{
    private readonly DataContext _dataContext;
    private readonly IClaimsProvider _claimsProvider;
    private readonly TimeProvider _timeProvider;

    public UpdateSubscriptionHandler(DataContext dataContext, IClaimsProvider claimsProvider, TimeProvider timeProvider)
    {
        _dataContext = dataContext ;
        _claimsProvider = claimsProvider;
        _timeProvider = timeProvider;
    }

    public async Task Execute(UpdateSubscriptionCommand command)
    {
        Guid userId = _claimsProvider.GetCurrentUserId();
        Subscription? subscription = await _dataContext.Subscriptions.FirstOrDefaultAsync(s => s.Id == command.Id);
        if (subscription == null)
            throw new NotFoundPostGramException($"Not found subscription with id: {command.Id}");

        if (subscription.MasterId != userId && subscription.SlaveId != userId)
            throw new AuthorizationPostGramException("Con not modify subscription of another user");

        if (subscription.SlaveId == userId && command.Status)
            throw new AuthorizationPostGramException(
                "Can not confirm subscription being slave to this subscription");

        subscription.Status = command.Status;
        subscription.Edited = _timeProvider.GetUtcNow();

        await _dataContext.SaveChangesAsync();
    }
}
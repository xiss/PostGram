using Microsoft.EntityFrameworkCore;
using PostGram.BLL.Interfaces.Base.Commands;
using PostGram.BLL.Interfaces.Providers;
using PostGram.Common.Exceptions;
using PostGram.Common.Requests.Commands;
using PostGram.DAL;
using PostGram.DAL.Entities;

namespace PostGram.BLL.Handlers.Commands;

public class CreateSubscriptionHandler : ICommandHandler<CreateSubscriptionCommand>
{
    private readonly DataContext _dataContext;
    private readonly IClaimsProvider _claimsProvider;

    public CreateSubscriptionHandler(DataContext dataContext, IClaimsProvider claimsProvider)
    {
        _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        _claimsProvider = claimsProvider ?? throw new ArgumentNullException(nameof(claimsProvider));
    }

    public async Task Execute(CreateSubscriptionCommand command)
    {
        Guid userId = _claimsProvider.GetCurrentUserId();
        if (!await _dataContext.Users.AnyAsync(u => u.Id == command.MasterId))
            throw new NotFoundPostGramException($"User {command.MasterId} not found");

        if (await _dataContext.Subscriptions.AnyAsync(s => s.MasterId == command.MasterId && s.SlaveId == userId))
            throw new UnprocessableRequestPostGramException(
                $"Subscription with masterId {command.MasterId} and slaveId {userId} is already exist");

        Subscription subscription = new()
        {
            Id = Guid.NewGuid(),
            Created = DateTimeOffset.UtcNow,
            SlaveId = userId,
            MasterId = command.MasterId,
            Status = false
        };

        await _dataContext.Subscriptions.AddAsync(subscription);
        await _dataContext.SaveChangesAsync();
    }
}
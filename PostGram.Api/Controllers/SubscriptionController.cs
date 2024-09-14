using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostGram.BLL.Interfaces.Base.Commands;
using PostGram.BLL.Interfaces.Base.Queries;
using PostGram.Common.Requests.Commands;
using PostGram.Common.Requests.Queries;
using PostGram.Common.Results;

namespace PostGram.Api.Controllers;

[ApiExplorerSettings(GroupName = Common.Constants.Api.EndpointApiName)]
[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
public class SubscriptionController : ControllerBase
{
    private readonly ICommandHandler<CreateSubscriptionCommand> _createSubscriptionHandler;
    private readonly ICommandHandler<UpdateSubscriptionCommand> _updateSubscriptionHandler;
    private readonly IQueryHandler<GetMasterSubscriptionsQuery, GetMasterSubscriptionsResult> _getMasterSubscriptionsHandler;
    private readonly IQueryHandler<GetSlaveSubscriptionsQuery, GetSlaveSubscriptionsResult> _getSlaveSubscriptionsHandler;

    public SubscriptionController(ICommandHandler<CreateSubscriptionCommand> createSubscriptionHandler,
        ICommandHandler<UpdateSubscriptionCommand> updateSubscriptionHandler,
        IQueryHandler<GetMasterSubscriptionsQuery, GetMasterSubscriptionsResult> getMasterSubscriptionsHandler,
        IQueryHandler<GetSlaveSubscriptionsQuery, GetSlaveSubscriptionsResult> getSlaveSubscriptionsHandler)
    {
        _createSubscriptionHandler = createSubscriptionHandler ?? throw new ArgumentNullException(nameof(createSubscriptionHandler));
        _updateSubscriptionHandler = updateSubscriptionHandler ?? throw new ArgumentNullException(nameof(updateSubscriptionHandler));
        _getMasterSubscriptionsHandler = getMasterSubscriptionsHandler
            ?? throw new ArgumentNullException(nameof(getMasterSubscriptionsHandler));
        _getSlaveSubscriptionsHandler = getSlaveSubscriptionsHandler
            ?? throw new ArgumentNullException(nameof(getSlaveSubscriptionsHandler));
    }

    [HttpPost]
    public async Task CreateSubscription(CreateSubscriptionCommand command)
    {
        await _createSubscriptionHandler.Execute(command);
    }

    [HttpGet]
    public async Task<GetMasterSubscriptionsResult> GetMasterSubscriptions(GetMasterSubscriptionsQuery query)
    {
        return await _getMasterSubscriptionsHandler.Execute(query);
    }

    [HttpGet]
    public async Task<GetSlaveSubscriptionsResult> GetSlaveSubscriptions(GetSlaveSubscriptionsQuery query)
    {
        return await _getSlaveSubscriptionsHandler.Execute(query);
    }

    [HttpPut]
    public async Task UpdateSubscription(UpdateSubscriptionCommand command)
    {
        await _updateSubscriptionHandler.Execute(command);
    }
}
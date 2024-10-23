using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostGram.Common.Constants;
using PostGram.Common.Interfaces.Base.Commands;
using PostGram.Common.Interfaces.Base.Queries;
using PostGram.Common.Requests.Commands;
using PostGram.Common.Requests.Queries;
using PostGram.Common.Results;

namespace PostGram.Api.Controllers;

[ApiExplorerSettings(GroupName = EndpointApiNames.Api)]
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
        _createSubscriptionHandler = createSubscriptionHandler ;
        _updateSubscriptionHandler = updateSubscriptionHandler;
        _getMasterSubscriptionsHandler = getMasterSubscriptionsHandler            ;
        _getSlaveSubscriptionsHandler = getSlaveSubscriptionsHandler            ;
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
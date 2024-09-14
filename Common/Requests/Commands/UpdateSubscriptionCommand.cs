using PostGram.BLL.Interfaces.Base.Commands;

namespace PostGram.Common.Requests.Commands;

public record UpdateSubscriptionCommand : ICommand
{
    public Guid Id { get; init; }
    public bool Status { get; init; }
}
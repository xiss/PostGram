namespace PostGram.Common.Requests;

public record UpdateSubscriptionModel
{
    public Guid Id { get; init; }
    public bool Status { get; init; }
}
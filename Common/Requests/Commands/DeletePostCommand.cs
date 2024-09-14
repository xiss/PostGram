using PostGram.BLL.Interfaces.Base.Commands;

namespace PostGram.Common.Requests.Commands;

public record DeletePostCommand : ICommand
{
    public Guid PostId { get; init; }
}
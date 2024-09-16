using PostGram.Common.Dtos.Attachment;
using PostGram.Common.Interfaces.Base.Commands;

namespace PostGram.Common.Requests.Commands;

public record AddAvatarToUserCommand : ICommand
{
   public MetadataDto Metadata { get; init; }
}
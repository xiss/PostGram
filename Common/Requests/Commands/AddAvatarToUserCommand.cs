using PostGram.BLL.Interfaces.Base.Commands;
using PostGram.Common.Dtos.Attachment;

namespace PostGram.Common.Requests.Commands;

public record AddAvatarToUserCommand : ICommand
{
   public MetadataDto Metadata { get; init; }
}
using PostGram.Common.Dtos.Attachment;
using PostGram.Common.Interfaces.Base.Queries;

namespace PostGram.Common.Requests.Queries;

public record GetAvatarForUserQuery : IQuery<FileInfoDto>
{
    public Guid UserId { get; init; }
    public bool IsDownload { get; init; }
}
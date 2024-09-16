using PostGram.Common.Dtos.Attachment;
using PostGram.Common.Interfaces.Base.Queries;

namespace PostGram.Common.Requests.Queries;

public record GetPostContentQuery : IQuery<FileInfoDto>
{
    public Guid PostContentId { get; init; }
    public bool IsDownload { get; init; }
}
using PostGram.Common.Dtos.Attachment;

namespace PostGram.Common.Dtos;

public record PostDto
{
    public UserDto Author { get; init; } = new UserDto();
    public Guid Id { get; init; }
    public DateTimeOffset Created { get; init; }
    public DateTimeOffset? Edited { get; init; }
    public string Header { get; init; } = string.Empty;
    public string Body { get; init; } = string.Empty;
    public int LikeCount { get; init; }
    public int DislikeCount { get; init; }
    public int CommentCount { get; init; }
    public LikeDto? LikeByUser { get; set; }
    public ICollection<AttachmentDto> Content { get; init; } = new List<AttachmentDto>();
}
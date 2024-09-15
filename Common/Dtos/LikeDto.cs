using PostGram.Common.Enums;

namespace PostGram.Common.Dtos;

public record LikeDto
{
    public Guid Id { get; init; }
    public bool? IsLike { get; init; }
    public Guid EntityId { get; init; }
    public Guid AuthorId { get; init; }
    public DateTimeOffset Created { get; init; }
    public LikableEntities EntityType { get; init; }
}
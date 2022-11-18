using PostGram.Common.Enums;

namespace PostGram.Api.Models.Like
{
    public class LikeModel
    {
        public Guid Id { get; set; }
        public bool? IsLike { get; set; }
        public Guid EntityId { get; set; }
        public Guid AuthorId { get; set; }
        public DateTimeOffset Created { get; set; }
        public LikableEntities EntityType { get; set; }
    }
}
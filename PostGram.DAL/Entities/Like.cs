using PostGram.Common.Enums;

namespace PostGram.DAL.Entities
{
    public class Like : CreationBase
    {
        public bool? IsLike { get; set; }
        public Guid EntityId { get; set; }
        public LikableEntities EntityType { get; set; }
    }
}
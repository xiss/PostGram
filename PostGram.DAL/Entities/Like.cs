using PostGram.Common.Enums;

namespace PostGram.DAL.Entities
{
    public class Like 
    {
        //TODO разделить на две Reaction tabel
        public Guid Id { get; set; }
        public Guid AuthorId { get; set; }
        public virtual User Author { get; set; } = null!;
        public DateTimeOffset Created { get; set; }
        public bool? IsLike { get; set; }
        public Guid EntityId { get; set; }
        public LikableEntities EntityType { get; set; }
    }
}
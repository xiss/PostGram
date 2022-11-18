namespace PostGram.DAL.Entities
{
    public class Like : CreationBase
    {
        public bool? IsLike { get; set; }
        public Guid? PostId { get; set; }
        public virtual Comment? Comment { get; set; }
        public Guid? CommentId { get; set; }
        public virtual Post? Post { get; set; }
    }
}
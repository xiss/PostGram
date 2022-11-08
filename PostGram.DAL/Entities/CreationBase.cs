namespace PostGram.DAL.Entities
{
    public abstract class CreationBase
    {
        public Guid Id { get; set; }
        public Guid AuthorId { get; set; }
        public virtual User Author { get; set; } = null!;
        public DateTimeOffset Created { get; set; }
    }
}
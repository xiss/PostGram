namespace PostGram.DAL.Entities
{
    public class Attachment
    {
        public Guid Id { get; set; }
        public Guid AuthorId { get; set; }
        public virtual User Author { get; set; } = null!;
        public DateTimeOffset Created { get; set; }
        public string Name { get; set; } = null!;
        public string MimeType { get; set; } = null!;
        public long Size { get; set; }
    }
}
namespace PostGram.DAL.Entities
{
    public class Attachment : CreationBase
    {
        public string Name { get; set; } = null!;
        public string MimeType { get; set; } = null!;
        public long Size { get; set; }
        public string FilePath { get; set; } = null!;
    }
}
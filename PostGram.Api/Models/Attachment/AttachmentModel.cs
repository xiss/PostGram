namespace PostGram.Api.Models.Attachment
{
    public class AttachmentModel 
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string MimeType { get; set; } = string.Empty;
        public string? Link { get; set; }
    }
}
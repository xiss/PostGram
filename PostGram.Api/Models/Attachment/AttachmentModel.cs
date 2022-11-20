namespace PostGram.Api.Models.Attachment
{
    public record AttachmentModel 
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string MimeType { get; init; } = string.Empty;
        public string? Link { get; set; }
    }
}
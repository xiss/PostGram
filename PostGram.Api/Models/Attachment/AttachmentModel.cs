namespace PostGram.Api.Models.Attachment
{
    public class AttachmentModel
    {
        public string Name { get; set; } = null!;
        public string MimeType { get; set; } = null!;
        public string FilePath { get; set; } = null!;
    }
}
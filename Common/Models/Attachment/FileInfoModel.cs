namespace PostGram.Common.Models.Attachment
{
    public record FileInfoModel()
    {
        public string Name { get; init; } = string.Empty;
        public string MimeType { get; init; } = string.Empty;
        public string Path { get; init; } = string.Empty;
    }
}
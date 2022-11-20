namespace PostGram.Api.Models.Attachment
{
    public record FileInfoModel(string Name, string MimeType, string Path)
    {
        public string Name { get; init; } = Name ?? throw new ArgumentNullException(nameof(Name));
        public string MimeType { get; init; } = MimeType ?? throw new ArgumentNullException(nameof(MimeType));
        public string Path { get; init; } = Path ?? throw new ArgumentNullException(nameof(Path));
    }
}
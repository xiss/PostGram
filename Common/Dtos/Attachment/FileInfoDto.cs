namespace PostGram.Common.Dtos.Attachment;

public record FileInfoDto()
{
    public string Name { get; init; } = string.Empty;
    public string MimeType { get; init; } = string.Empty;
    //public string Path { get; init; } = string.Empty;
    public FileStream FileStream { get; init; }
}
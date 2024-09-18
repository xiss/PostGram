namespace PostGram.Common.Dtos.Attachment;

public record AttachmentDto
{
    //TODO Убрать выбрасывание исключений
    //TODO Добавить логирование
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string MimeType { get; init; } = string.Empty;
    public string? Link { get; set; }
}
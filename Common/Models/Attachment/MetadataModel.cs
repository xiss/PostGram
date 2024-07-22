using System.ComponentModel.DataAnnotations;

namespace PostGram.Common.Models.Attachment
{
    public record MetadataModel
    {
        [Required]
        public Guid TempId { get; init; }
        [Required]
        public string Name { get; init; } = string.Empty;
        [Required]
        public string MimeType { get; init; } = string.Empty;
        [Required]
        public long Size { get; init; }
    }
}
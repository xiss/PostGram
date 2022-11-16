using System.ComponentModel.DataAnnotations;

namespace PostGram.Api.Models.Attachment
{
    public class MetadataModel
    {
        [Required]
        public Guid TempId { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string MimeType { get; set; } = null!;
        [Required]
        public long Size { get; set; }
    }
}
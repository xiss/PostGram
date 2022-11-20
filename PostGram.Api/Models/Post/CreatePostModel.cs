using System.ComponentModel.DataAnnotations;
using PostGram.Api.Models.Attachment;

namespace PostGram.Api.Models.Post
{
    public record CreatePostModel
    {
        [Required]
        public string Header { get; init; } = string.Empty;

        [Required]
        public string Body { get; init; } = string.Empty;

        [Required]
        public virtual ICollection<MetadataModel> Attachments { get; init; } = new List<MetadataModel>();
    }
}
using System.ComponentModel.DataAnnotations;
using PostGram.Common.Constants;
using PostGram.Common.Models.Attachment;

namespace PostGram.Common.Models.Post
{
    public record UpdatePostModel
    {
        [Required]
        public Guid Id { get; init; }
        [StringLength(ModelValidation.PostHeaderLength)]
        public string? UpdatedHeader { get; init; } = null;
        [StringLength(ModelValidation.PostBodyLength)]
        public string? UpdatedBody { get; init; } = null;
        public ICollection<MetadataModel> NewContent { get; init; } = new List<MetadataModel>();
        public ICollection<AttachmentModel> ContentToDelete { get; init; } = new List<AttachmentModel>();
    }
}
using PostGram.Api.Models.Attachment;
using PostGram.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace PostGram.Api.Models.Post
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
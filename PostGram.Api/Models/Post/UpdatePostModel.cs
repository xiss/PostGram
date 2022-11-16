using PostGram.Api.Models.Attachment;
using System.ComponentModel.DataAnnotations;

namespace PostGram.Api.Models.Post
{
    public class UpdatePostModel
    {
        [Required]
        public Guid Id { get; set; }
        public string? UpdatedHeader { get; set; } = null;
        public string? UpdatedBody { get; set; } = null;
        public ICollection<MetadataModel> NewContent { get; set; } = new List<MetadataModel>();
        public ICollection<AttachmentModel> ContentToDelete { get; set; } = new List<AttachmentModel>();
    }
}
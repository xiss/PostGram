using System.ComponentModel.DataAnnotations;
using PostGram.Api.Models.Attachment;

namespace PostGram.Api.Models.Post
{
    public class CreatePostModel
    {
        [Required]
        public string Header { get; set; } = null!;

        [Required]
        public string Body { get; set; } = null!;

        [Required]
        public virtual ICollection<MetadataModel> Attachments { get; set; } = new List<MetadataModel>();
    }
}
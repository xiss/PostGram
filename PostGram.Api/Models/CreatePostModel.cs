using System.ComponentModel.DataAnnotations;
using PostGram.DAL.Entities;

namespace PostGram.Api.Models
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

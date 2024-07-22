using System.ComponentModel.DataAnnotations;
using PostGram.Common.Models.Attachment;

namespace PostGram.Common.Models.Avatar
{
    public record AddAvatarRequestModel
    {
        [Required]
        public MetadataModel Avatar { get; init; } = new ();
        [Required]
        public Guid UserId { get; init; }
    }
}
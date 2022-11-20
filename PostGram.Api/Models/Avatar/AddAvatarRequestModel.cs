using PostGram.Api.Models.Attachment;
using System.ComponentModel.DataAnnotations;

namespace PostGram.Api.Models.Avatar
{
    public record AddAvatarRequestModel
    {
        [Required]
        public MetadataModel Avatar { get; init; } = new ();
        [Required]
        public Guid UserId { get; init; }
    }
}
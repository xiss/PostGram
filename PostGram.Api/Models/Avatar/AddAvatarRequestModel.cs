using PostGram.Api.Models.Attachment;

namespace PostGram.Api.Models.Avatar
{
    public class AddAvatarRequestModel
    {
        public MetadataModel Avatar { get; set; } = null!;

        public Guid UserId { get; set; }
    }
}
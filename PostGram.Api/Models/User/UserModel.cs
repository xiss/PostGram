using PostGram.Api.Models.Attachment;

namespace PostGram.Api.Models.User
{
    public record UserModel 
    {
        public AttachmentModel? Avatar { get; init; }
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Surname { get; init; } = string.Empty;
        public string Patronymic { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Nickname { get; init; } = string.Empty;
        public DateTimeOffset? BirthDate { get; init; }
    }
}
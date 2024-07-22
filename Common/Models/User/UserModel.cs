using PostGram.Common.Models.Attachment;

namespace PostGram.Common.Models.User
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
        public bool IsDelete { get; init; }
        public bool IsPrivate { get; set; }
    }
}
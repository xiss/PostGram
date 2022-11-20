using PostGram.Api.Models.Attachment;

namespace PostGram.Api.Models.User
{
    public record UserModel 
    {
        public AttachmentModel? Avatar { get; init; }
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Surname { get; init; }
        public string Patronymic { get; init; }
        public string Email { get; init; }
        public string Nickname { get; init; }
        public DateTimeOffset? BirthDate { get; init; }
    }
}
using PostGram.Api.Models.Attachment;

namespace PostGram.Api.Models.User
{
    public class UserModel 
    {
        public AttachmentModel? Avatar { get; set; }
        //public Guid AvatarId { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }
        public DateTimeOffset? BirthDate { get; set; }

    }
}
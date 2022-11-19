using System.ComponentModel.DataAnnotations;

namespace PostGram.Api.Models.User
{
    public class UpdateUserModel
    {
        [Required]
        public Guid UserId { get; set; }
        public DateTimeOffset? NewBirthDate { get; set; }
        public bool? NewIsPrivate { get; set; } = true;
        public string? NewName { get; set; }
        public string? NewNickname { get; set; }
        public string? NewPatronymic { get; set; }
        public string? NewSurname { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace PostGram.Api.Models.User
{
    public record UpdateUserModel
    {
        [Required]
        public Guid UserId { get; init; }
        public DateTimeOffset? NewBirthDate { get; init; }
        public bool? NewIsPrivate { get; init; } 
        public string? NewName { get; init; }
        public string? NewNickname { get; init; }
        public string? NewPatronymic { get; init; }
        public string? NewSurname { get; init; }
    }
}
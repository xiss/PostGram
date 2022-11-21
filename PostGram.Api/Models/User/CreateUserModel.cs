using System.ComponentModel.DataAnnotations;

namespace PostGram.Api.Models.User
{
    public record CreateUserModel
    {
        //todo ddos auth
        public DateTimeOffset BirthDate { get; init; }

        [Required]
        [EmailAddress]
        public string Email { get; init; } = string.Empty;

        public bool IsPrivate { get; init; } = true;
        public string Name { get; init; } = string.Empty;
        public string Nickname { get; init; } = string.Empty;

        [Required]
        [Compare(nameof(PasswordRetry))]
        public string Password { get; init; } = string.Empty;

        [Required]
        public string PasswordRetry { get; init; } = string.Empty;

        public string Patronymic { get; init; } = string.Empty;
        public string Surname { get; init; } = string.Empty;
    }
}
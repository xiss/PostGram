using PostGram.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace PostGram.Api.Models.User
{
    public record CreateUserModel
    {
        public DateTimeOffset BirthDate { get; init; }

        [Required]
        [EmailAddress]
        public string Email { get; init; } = string.Empty;

        public bool IsPrivate { get; init; } = true;
        [StringLength(ModelValidation.UserNameLength)]
        public string Name { get; init; } = string.Empty;
        [StringLength(ModelValidation.UserNicknameLength)]
        public string Nickname { get; init; } = string.Empty;

        [Required]
        [Compare(nameof(PasswordRetry))]
        [StringLength(ModelValidation.UserPasswordLength)]
        public string Password { get; init; } = string.Empty;
        [StringLength(ModelValidation.UserPasswordLength)]

        [Required]
        public string PasswordRetry { get; init; } = string.Empty;
        [StringLength(ModelValidation.UserPatronymicLength)]
        public string Patronymic { get; init; } = string.Empty;
        [StringLength(ModelValidation.UserSurnameLength)]
        public string Surname { get; init; } = string.Empty;
    }
}
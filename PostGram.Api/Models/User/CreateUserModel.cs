using System.ComponentModel.DataAnnotations;

namespace PostGram.Api.Models.User
{
    public record CreateUserModel
    {
        public CreateUserModel(string name, string surname, string patronymic, string email, string nickname, string password, string passwordRetry, DateTimeOffset birthDate)
        {
            Name = name;
            Surname = surname;
            Patronymic = patronymic;
            Email = email;
            Nickname = nickname;
            Password = password;
            PasswordRetry = passwordRetry;
            BirthDate = birthDate;
        }

        // TODO 3 Сделать проверку что дата не в будущем времени
        public DateTimeOffset BirthDate { get; init; }

        [Required]
        [EmailAddress]
        public string Email { get; init; }

        public bool IsPrivate { get; init; } = true;
        public string Name { get; init; }
        public string Nickname { get; init; }

        [Required]
        [Compare(nameof(PasswordRetry))]
        public string Password { get; init; }

        [Required]
        public string PasswordRetry { get; init; }

        public string Patronymic { get; init; }
        public string Surname { get; init; }
    }
}
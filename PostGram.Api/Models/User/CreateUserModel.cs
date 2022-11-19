using System.ComponentModel.DataAnnotations;

namespace PostGram.Api.Models.User
{
    public class CreateUserModel
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
        public DateTimeOffset BirthDate { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public bool IsPrivate { get; set; } = true;
        public string Name { get; set; }
        public string Nickname { get; set; }

        [Required]
        [Compare(nameof(PasswordRetry))]
        public string Password { get; set; }

        [Required]
        public string PasswordRetry { get; set; }

        public string Patronymic { get; set; }
        public string Surname { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace PostGram.Api.Models
{
    public class CreateUserModel
    {
        public CreateUserModel(string name, string surname, string patronymic, string email, string login, string password, string passwordRetry, DateTimeOffset birthDate)
        {
            Name = name;
            Surname = surname;
            Patronymic = patronymic;
            Email = email;
            Login = login;
            Password = password;
            PasswordRetry = passwordRetry;
            BirthDate = birthDate;
        } 

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Login { get; set; }
        [Required]
        [Compare(nameof(PasswordRetry))]
        public string Password { get; set; }
        [Required]
        public string PasswordRetry { get; set; }
        public DateTimeOffset BirthDate { get; set; }
    }
}

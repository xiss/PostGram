using System.ComponentModel.DataAnnotations;
using System;
using System.Runtime.CompilerServices;
using PostGram.DAL.Entities;

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
        [EmailAddress]
        public string Email { get; set; }
        public string Login { get; set; }
        [Required]
        [Compare(nameof(PasswordRetry))]
        public string Password { get; set; }
        [Required]
        public string PasswordRetry { get; set; }

        // TODO 3 Сделать проверку что дата не в будущем времени
        public DateTimeOffset BirthDate { get; set; }
    }
}

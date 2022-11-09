namespace PostGram.Api.Models.User
{
    public class UserModel
    {
        public UserModel(Guid id, string name, string surname, string patronymic, string email, string login, DateTimeOffset birthDate)
        {
            Id = id;
            Name = name;
            Surname = surname;
            Patronymic = patronymic;
            Email = email;
            Login = login;
            BirthDate = birthDate;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }
        public DateTimeOffset BirthDate { get; set; }
    }
}
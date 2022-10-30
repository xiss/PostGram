using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostGram.DAL.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "empty";
        public string Surname { get; set; } = "empty";
        public string Patronymic { get; set; } = "empty";
        public string Email { get; set; } = "empty";
        public string Login { get; set; } = "empty";
        public string PasswordHash { get; set; } = "empty";
        public  DateTimeOffset BirthDate { get; set; }
    }
}

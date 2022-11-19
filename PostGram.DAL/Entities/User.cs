﻿namespace PostGram.DAL.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "empty";
        public string Surname { get; set; } = "empty";
        public string Patronymic { get; set; } = "empty";
        public string Email { get; set; } = "empty";
        public string Nickname { get; set; } = "empty";
        public string PasswordHash { get; set; } = "empty";
        public DateTimeOffset BirthDate { get; set; }
        public virtual Avatar? Avatar { get; set; }
        public Guid? AvatarId { get; set; }
        public bool IsPrivate { get; set; } = true;
        public virtual ICollection<UserSession>? Sessions { get; set; } = new List<UserSession>();
        public virtual ICollection<Subscription> Slaves { get; set; } = new List<Subscription>();
        public virtual ICollection<Subscription> Masters { get; set; } = new List<Subscription>();

    }
}
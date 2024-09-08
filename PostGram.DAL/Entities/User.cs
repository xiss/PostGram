namespace PostGram.DAL.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Patronymic { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTimeOffset BirthDate { get; set; }
    public virtual Avatar? Avatar { get; set; }
    public Guid? AvatarId { get; set; }
    public bool IsPrivate { get; set; } = true;
    public bool IsDelete { get; set; } = false;
    public virtual ICollection<UserSession>? Sessions { get; set; } = new List<UserSession>();
    public virtual ICollection<Subscription> Slaves { get; set; } = new List<Subscription>();
    public virtual ICollection<Subscription> Masters { get; set; } = new List<Subscription>();

}
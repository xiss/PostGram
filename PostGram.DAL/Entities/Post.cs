namespace PostGram.DAL.Entities;

public class Post
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public virtual User Author { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public string Header { get; set; } = null!;
    public string Body { get; set; } = null!;
    public DateTimeOffset? Edited { get; set; }
    public bool IsDeleted { get; set; } = false;
    public virtual ICollection<PostContent> PostContents { get; set; } = new List<PostContent>();
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
}
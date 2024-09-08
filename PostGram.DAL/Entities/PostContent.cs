namespace PostGram.DAL.Entities;

public class PostContent : Attachment
{
    public Guid PostId { get; set; }
    public virtual Post Post { get; set; } = null!;
}
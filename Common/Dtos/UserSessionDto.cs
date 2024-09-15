namespace PostGram.Common.Dtos;

public record UserSessionDto
{
    public Guid Id { get; set; }
    public Guid RefreshTokenId { get; set; }
    public Guid UserId { get; set; }
    public virtual UserDto User { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public bool IsActive { get; set; } = true;
}
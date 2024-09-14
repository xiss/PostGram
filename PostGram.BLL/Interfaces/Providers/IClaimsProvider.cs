namespace PostGram.BLL.Interfaces.Providers;

public interface IClaimsProvider
{
    Guid GetCurrentUserId();

    Guid GetCurrentSessionId();
}
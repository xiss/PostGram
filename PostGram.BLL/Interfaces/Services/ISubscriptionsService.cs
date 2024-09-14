namespace PostGram.BLL.Interfaces.Services;

public interface ISubscriptionsService
{
    Task<List<Guid>> GetAvailableSubscriptionsForSlaveUser(Guid slaveUserId);
}
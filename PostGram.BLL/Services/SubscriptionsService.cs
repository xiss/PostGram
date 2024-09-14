using Microsoft.EntityFrameworkCore;
using PostGram.BLL.Interfaces.Services;
using PostGram.DAL;

namespace PostGram.BLL.Services;

//TODO Вопрос. Стоит ли выносить контекст из обработчиков? Если да, то нужно делать под каждую сущбность свой сервис через которую в контекст лазить?
public class SubscriptionsService : ISubscriptionsService
{
    private readonly DataContext _dataContext;

    public SubscriptionsService(DataContext dataContext)
    {
        _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
    }

    public async Task<List<Guid>> GetAvailableSubscriptionsForSlaveUser(Guid slaveUserId)
    {
        return await _dataContext.Subscriptions
            .Where(s => s.SlaveId == slaveUserId && (s.Status || !s.Master.IsPrivate))
            .Select(s => s.MasterId)
            .ToListAsync();
    }
}
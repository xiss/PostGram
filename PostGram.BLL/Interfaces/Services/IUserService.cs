using PostGram.DAL.Entities;

namespace PostGram.BLL.Interfaces.Services;

public interface IUserService
{
    Task<User> GetUserById(Guid id);
    Task<bool> CheckUserExist(string email);
}
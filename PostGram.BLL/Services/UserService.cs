using Microsoft.EntityFrameworkCore;
using PostGram.BLL.Interfaces.Services;
using PostGram.Common.Exceptions;
using PostGram.DAL;
using PostGram.DAL.Entities;

namespace PostGram.BLL.Services;

public class UserService : IUserService
{
    private readonly DataContext _dataContext;

    public UserService(DataContext dataContext)
    {
        _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
    }

    public async Task<User> GetUserById(Guid id)
    {
        User? user = await _dataContext.Users
            .Include(u => u.Avatar)
            .FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
            throw new NotFoundPostGramException("User not found, userId: " + id);
        return user;
    }

    public async Task<bool> CheckUserExist(string email)
    {
        return await _dataContext.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
    }
}
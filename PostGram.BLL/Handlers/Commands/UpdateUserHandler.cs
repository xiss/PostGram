using PostGram.BLL.Interfaces.Providers;
using PostGram.BLL.Interfaces.Services;
using PostGram.Common.Exceptions;
using PostGram.Common.Interfaces.Base.Commands;
using PostGram.Common.Requests.Commands;
using PostGram.DAL;
using PostGram.DAL.Entities;

namespace PostGram.BLL.Handlers.Commands;

public class UpdateUserHandler : ICommandHandler<UpdateUserCommand>
{
    private readonly DataContext _dataContext;
    private readonly IClaimsProvider _claimsProvider;
    private readonly IUserService _userService;

    public UpdateUserHandler(DataContext dataContext, IClaimsProvider claimsProvider, IUserService userService)
    {
        _dataContext = dataContext;
        _claimsProvider = claimsProvider;
        _userService = userService;
    }

    public async Task Execute(UpdateUserCommand command)
    {
        Guid userId = _claimsProvider.GetCurrentUserId();

        User user = await _userService.GetUserById(command.UserId);

        //TODO Куда убрать валидацию?
        if (user.Id != userId)
            throw new AuthorizationPostGramException("Cannot modify another user");
        if (command.NewBirthDate != null)
            user.BirthDate = command.NewBirthDate.Value;
        if (command.NewIsPrivate != null)
            user.IsPrivate = command.NewIsPrivate.Value;
        if (command.NewName != null)
            user.Name = command.NewName;
        if (command.NewNickname != null)
            user.Nickname = command.NewNickname;
        if (command.NewPatronymic != null)
            user.Patronymic = command.NewPatronymic;
        if (command.NewSurname != null)
            user.Surname = command.NewSurname;

        await _dataContext.SaveChangesAsync();
    }
}
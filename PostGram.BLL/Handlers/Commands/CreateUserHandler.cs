using AutoMapper;
using PostGram.BLL.Interfaces.Services;
using PostGram.Common.Exceptions;
using PostGram.Common.Interfaces.Base.Commands;
using PostGram.Common.Requests.Commands;
using PostGram.DAL;
using PostGram.DAL.Entities;

namespace PostGram.BLL.Handlers.Commands;

public class CreateUserHandler : ICommandHandler<CreateUserCommand>
{
    private readonly DataContext _dataContext;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public CreateUserHandler(DataContext dataContext, IUserService userService, IMapper mapper)
    {
        _dataContext = dataContext ;
        _userService = userService;
        _mapper = mapper;
    }

    public async Task Execute(CreateUserCommand command)
    {
        if (await _userService.CheckUserExist(command.Email))
            throw new UnprocessableRequestPostGramException("User with email already exist, email: " + command.Email);
        if (command.BirthDate > DateTimeOffset.UtcNow)
            throw new BadRequestPostGramException("BirthDate cannot be in future");

        User user = _mapper.Map<User>(command);

        await _dataContext.Users.AddAsync(user);
        await _dataContext.SaveChangesAsync();
    }
}
namespace PostGram.Common.Interfaces.Base.Commands;

public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    Task Execute(TCommand command);
}
namespace PostGram.Common.Interfaces.Base.Commands;

public interface ICommandService<in TCommand> where TCommand : ICommand
{
    Task Execute(TCommand command);
}
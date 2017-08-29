namespace Shriek.Commands
{
    public interface ICommandHandler<TCommand> where TCommand : Command
    {
        void Execute(ICommandContext context, TCommand command);
    }
}
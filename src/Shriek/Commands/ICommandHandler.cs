namespace Shriek.Commands
{
    public interface ICommandHandler<in TCommand> where TCommand : Command
    {
        void Execute(ICommandContext context, TCommand command);
    }
}
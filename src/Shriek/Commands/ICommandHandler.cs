using Shriek.Messages;

namespace Shriek.Commands
{
    public interface ICommandHandler<in TCommand> where TCommand : Message
    {
        void Execute(ICommandContext context, TCommand command);
    }
}
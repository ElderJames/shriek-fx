using System.Collections.Concurrent;
using Shriek.Messages;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Shriek.Commands
{
    /// <summary>
    /// 命令总线
    /// </summary>
    public class CommandBus : ICommandBus
    {
        private IMessagePublisher messageProcessor;

        private readonly ConcurrentQueue<Command> commandQueue = new ConcurrentQueue<Command>();

        private static Task task;

        public CommandBus(IMessagePublisher messageProcessor)
        {
            this.messageProcessor = messageProcessor;
        }

        /// <summary>
        /// 发送命令
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <param name="command"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Send<TCommand>(TCommand command) where TCommand : Command
        {
            if (command == null)
                return;

            commandQueue.Enqueue(command);

            if (task == null || task.IsCompleted)
                task = Task.Run(() =>
                 {
                     while (!commandQueue.IsEmpty && commandQueue.TryDequeue(out var cmd))
                         messageProcessor.Send(cmd);
                 });
        }
    }
}
using Shriek.Messages;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Shriek.Commands;

namespace Shriek.Events
{
    public class InMemoryEventBus : IEventBus, IDisposable
    {
        private readonly IMessagePublisher messageProcessor;
        private readonly ConcurrentQueue<Event> commandQueue = new ConcurrentQueue<Event>();

        private static Task task;

        public InMemoryEventBus(IMessagePublisher messageProcessor)
        {
            this.messageProcessor = messageProcessor;
        }

        public void Dispose()
        {
            messageProcessor.Dispose();
        }

        public void Publish<T>(T @event) where T : Event
        {
            if (@event == null) return;

            commandQueue.Enqueue(@event);
            if (task == null || task.IsCompleted)
                task = Task.Run(() =>
                {
                    while (!commandQueue.IsEmpty && commandQueue.TryDequeue(out var evt))
                        messageProcessor.Send(evt);
                });
        }
    }
}
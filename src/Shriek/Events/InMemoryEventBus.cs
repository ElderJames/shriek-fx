using Shriek.Messages;
using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Shriek.Events
{
    public class InMemoryEventBus : IEventBus, IDisposable
    {
        private readonly IMessagePublisher messageProcessor;
        private readonly ConcurrentDictionary<Guid, ConcurrentQueue<Event>> commandDict = new ConcurrentDictionary<Guid, ConcurrentQueue<Event>>();

        private static Task task;

        public InMemoryEventBus(IMessagePublisher messageProcessor)
        {
            this.messageProcessor = messageProcessor;
        }

        public void Dispose()
        {
            messageProcessor.Dispose();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Publish<T>(T @event) where T : Event
        {
            if (@event == null) return;

            var commandQueue = commandDict.GetOrAdd(@event.AggregateId, new ConcurrentQueue<Event>());
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
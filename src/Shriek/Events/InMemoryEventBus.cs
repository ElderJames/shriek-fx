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
        private readonly ConcurrentDictionary<Guid, ConcurrentQueue<Event>> eventQueueDict = new ConcurrentDictionary<Guid, ConcurrentQueue<Event>>();
        private readonly ConcurrentDictionary<Guid, Task> taskDict = new ConcurrentDictionary<Guid, Task>();

        private static Task _task;

        public InMemoryEventBus(IMessagePublisher messageProcessor)
        {
            this.messageProcessor = messageProcessor;
        }

        public void Dispose()
        {
            messageProcessor.Dispose();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Publish<TEvent>(TEvent @event) where TEvent : Event
        {
            if (@event == null) return;

            var eventQueue = eventQueueDict.GetOrAdd(@event.AggregateId, new ConcurrentQueue<Event>());
            eventQueue.Enqueue(@event);

            if (!taskDict.TryGetValue(@event.AggregateId, out var task) || task.IsCompleted || task.IsCanceled || task.IsFaulted)
            {
                task?.Dispose();

                taskDict[@event.AggregateId] = Task.Run(() =>
                {
                    while (!eventQueue.IsEmpty && eventQueue.TryDequeue(out var evt))
                        messageProcessor.Send(evt);
                });
            }
        }
    }
}
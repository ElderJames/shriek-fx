using Shriek.Messages;
using System;
using System.Threading.Tasks;

namespace Shriek.Events
{
    public class InMemoryEventBus : IEventBus, IDisposable
    {
        private IMessagePublisher messageProcessor;

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
            Task.Run(() => messageProcessor.Send(@event));
        }
    }
}
using Shriek.Events;
using System;
using System.Runtime.CompilerServices;

namespace Shriek.Messages.RabbitMQ
{
    public class RabbitMqEventBus : IEventBus, IDisposable
    {
        private readonly IMessagePublisher _messagePublisher;

        public RabbitMqEventBus(IMessagePublisher messagePublisher)
        {
            this._messagePublisher = messagePublisher;
        }

        public void Dispose()
        {
            _messagePublisher.Dispose();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Publish<TEvent>(TEvent @event) where TEvent : Event
        {
            if (@event == null)
                return;

            _messagePublisher.Send(@event);
        }
    }
}
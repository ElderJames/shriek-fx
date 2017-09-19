using Microsoft.Extensions.DependencyInjection;
using Shriek.Events;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Shriek.Messages
{
    public class EventMessageSubscriber<TEvent> : IMessageSubscriber<TEvent> where TEvent : Event
    {
        private IServiceProvider container;

        public EventMessageSubscriber(IServiceProvider container)
        {
            this.container = container;
        }

        public void Execute(TEvent @event)
        {
            var handlers = container.GetServices<IEventHandler<TEvent>>();

            if (handlers != null && handlers.Any())
                foreach (var eventHandler in handlers)
                {
                    eventHandler.Handle(@event);
                }
        }
    }
}
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Shriek.Messages;

namespace Shriek.Events
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

            handlers.AsParallel().ForAll(eventHandler => eventHandler.Handle(@event));
        }
    }
}
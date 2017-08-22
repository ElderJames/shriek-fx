using Shriek.Messages;
using System.Collections.Concurrent;
using Shriek.Storage;
using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using Shriek.Notifications;
using System.Runtime.CompilerServices;

namespace Shriek.Events
{
    public class InMemoryEventBus : IEventBus, IDisposable
    {
        private IServiceProvider Container;
        private IMessageProcessor messageProcessor;

        public InMemoryEventBus(IServiceProvider Container, IMessageProcessor messageProcessor)
        {
            this.Container = Container;
            this.messageProcessor = messageProcessor;
            messageProcessor.Subscriber(h => Handle((dynamic)h));
        }

        public void Dispose()
        {
            messageProcessor.Dispose();
        }

        public void Publish<T>(T @event) where T : Event
        {
            messageProcessor.Send(@event);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void Handle<T>(T @event) where T : Event
        {
            var handlers = Container.GetServices<IEventHandler<T>>();

            if (handlers != null && handlers.Any())
                foreach (var eventHandler in handlers)
                {
                    eventHandler.Handle(@event);
                }
        }
    }
}
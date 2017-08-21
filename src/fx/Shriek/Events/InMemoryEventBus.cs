using Shriek.Storage;
using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using Shriek.Notifications;

namespace Shriek.Events
{
    public class InMemoryEventBus : IEventBus, IDisposable
    {
        private IServiceProvider Container;
        private static object _lock = new object();
        private Queue<Event> eventQueue;
        private Task queueTask;

        public InMemoryEventBus(IServiceProvider Container)
        {
            this.Container = Container;

            InitQueuePublisher();
        }

        public void Publish<T>(T @event) where T : Event
        {
            eventQueue.Enqueue(@event);
        }

        private void Hanlde<T>(T @event) where T : Event
        {
            var handlers = Container.GetServices<IEventHandler<T>>();

            if (handlers != null && handlers.Any())
                foreach (var eventHandler in handlers)
                {
                    eventHandler.Handle(@event);
                }
        }

        public void InitQueuePublisher()
        {
            eventQueue = new Queue<Event>();
            queueTask = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Thread.Sleep(1000);
                    lock (_lock)
                    {
                        if (!eventQueue.Any()) continue;
                        var desEvent = (dynamic)eventQueue.Dequeue();
                        Hanlde(desEvent);
                    }
                }
            });
        }

        public void Dispose()
        {
            queueTask?.Dispose();
        }
    }
}
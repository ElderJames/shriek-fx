using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Shriek.Storage;
using Shriek.Notifications;

namespace Shriek.Events
{
    public class EventBus : IEventBus
    {
        public static Func<IServiceProvider> ContainerAccessor { get; set; }
        private static IServiceProvider Container => ContainerAccessor();

        //private readonly IEventStorage _eventStore;

        //public EventBus(IEventStorage eventStore)
        //{
        //    _eventStore = eventStore;
        //}

        public void Publish<T>(T @event) where T : Event
        {
            //if (!(@event is DomainNotification))
            //    _eventStore?.Save(@event);

            var handlers = Container.GetServices<IEventHandler<T>>();

            if (handlers != null && handlers.Any())
                foreach (var eventHandler in handlers)
                {
                    eventHandler.Handle(@event);
                }
            else
                throw new Exceptions.DomainException($"没找到事件{nameof(@event)}的处理类，或IOC没注册");
        }
    }
}
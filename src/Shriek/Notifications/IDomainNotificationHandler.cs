using Shriek.Events;
using System.Collections.Generic;

namespace Shriek.Notifications
{
    public interface IDomainNotificationHandler<TEvent> : IEventHandler<TEvent> where TEvent : IEvent
    {
        bool NotEmpty { get; }

        List<TEvent> Notifications { get; }
    }
}
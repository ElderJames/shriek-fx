using System.Collections.Generic;
using Shriek.Events;

namespace Shriek.Notifications
{
    public interface IDomainNotificationHandler<TEvent> : IEventHandler<TEvent> where TEvent : IEvent
    {
        bool NotEmpty { get; }

        List<TEvent> Notifications { get; }
    }
}
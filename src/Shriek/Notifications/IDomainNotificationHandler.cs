using Shriek.Events;
using System.Collections.Generic;

namespace Shriek.Notifications
{
    public interface IDomainNotificationHandler<T> : IEventHandler<T> where T : Event
    {
        bool NotEmpty { get; }

        List<T> Notifications { get; }
    }
}
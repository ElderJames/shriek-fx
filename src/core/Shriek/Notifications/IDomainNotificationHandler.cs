using Shriek.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shriek.Notifications
{
    public interface IDomainNotificationHandler<T> : IEventHandler<T> where T : Event
    {
        bool NotEmpty { get; }

        List<T> Notifications { get; }
    }
}
using System.Collections.Generic;
using System.Linq;

namespace Shriek.Notifications
{
    public class DomainNotificationHandler : IDomainNotificationHandler<DomainNotification>
    {
        public DomainNotificationHandler()
        {
            Notifications = new List<DomainNotification>();
        }

        public void Handle(DomainNotification message)
        {
            Notifications.Add(message);
        }

        public List<DomainNotification> Notifications { get; private set; }

        public bool NotEmpty => Notifications.Any();

        public void Dispose()
        {
            Notifications = new List<DomainNotification>();
        }
    }
}
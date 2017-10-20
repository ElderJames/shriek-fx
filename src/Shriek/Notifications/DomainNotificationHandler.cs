using System.Collections.Generic;
using System.Linq;

namespace Shriek.Notifications
{
    public class DomainNotificationHandler : IDomainNotificationHandler<DomainNotification>
    {
        private List<DomainNotification> notifications;

        public DomainNotificationHandler()
        {
            notifications = new List<DomainNotification>();
        }

        public void Handle(DomainNotification message)
        {
            notifications.Add(message);
        }

        public List<DomainNotification> Notifications
        {
            get => notifications;
        }

        public bool NotEmpty
        {
            get => Notifications.Any();
        }

        public void Dispose()
        {
            notifications = new List<DomainNotification>();
        }
    }
}
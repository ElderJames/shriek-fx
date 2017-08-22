using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shriek.Notifications;

namespace Shriek.Samples.InProcess.Notifications
{
    public class DomainNotificationHandler : IDomainNotificationHandler<DomainNotification>
    {
        private List<DomainNotification> _notifications;

        public DomainNotificationHandler()
        {
            _notifications = new List<DomainNotification>();
        }

        public void Handle(DomainNotification message)
        {
            Console.WriteLine("exception:" + message.Key + ":" + message.Value);
            _notifications.Add(message);
        }

        public List<DomainNotification> Notifications
        {
            get => _notifications;
        }

        public bool NotEmpty
        {
            get => Notifications.Any();
        }

        public void Dispose()
        {
            _notifications = new List<DomainNotification>();
        }
    }
}
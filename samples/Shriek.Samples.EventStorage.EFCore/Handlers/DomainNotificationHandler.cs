using System;
using System.Collections.Generic;
using System.Linq;
using Shriek.Notifications;

namespace Shriek.Samples.EventStorage.EFCore.Handlers
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
            Console.WriteLine("exception:" + message.Key + ":" + message.Value);
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
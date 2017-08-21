using System.Threading;
using Shriek.ConfigCenter.Domain.Events;
using Shriek.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shriek.ConfigCenter.Domain.Handlers
{
    public class ConfigItemEventHandler : IEventHandler<ConfigItemCreatedEvent>, IEventHandler<ConfigItemChangedEvent>
    {
        public void Handle(ConfigItemCreatedEvent e)
        {
            Console.WriteLine("here is " + e.Name);
            Thread.Sleep(5000);
            Console.WriteLine(e.Name + " finished!");
        }

        public void Handle(ConfigItemChangedEvent e)
        {
            Console.WriteLine("here is " + e.Name);
            Thread.Sleep(5000);
            Console.WriteLine(e.Name + " finished!");
        }
    }
}
using Shriek.ConfigCenter.Domain.Events;
using Shriek.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shriek.ConfigCenter.Domain.Handlers
{
    public class ConfigItemEventHandler : IEventHandler<ConfigItemCreatedEvent>
    {
        public void Handle(ConfigItemCreatedEvent e)
        {
            Console.WriteLine("here is " + e.GetType().Name);
        }
    }
}
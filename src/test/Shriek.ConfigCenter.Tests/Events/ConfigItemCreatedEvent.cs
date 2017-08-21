using Shriek.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shriek.Console.Events
{
    public class ConfigItemCreatedEvent : Event
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }
}
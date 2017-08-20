using System.Reflection.Metadata.Ecma335;
using Shriek.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shriek.ConfigCenter.Domain.Events
{
    public class ConfigItemChangedEvent : Event
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }
}
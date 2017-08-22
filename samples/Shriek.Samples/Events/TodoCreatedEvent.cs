using Shriek.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shriek.Samples.Events
{
    public class TodoCreatedEvent : Event
    {
        public string Name { get; set; }

        public string Desception { get; set; }
        public DateTime FinishTime { get; internal set; }
    }
}
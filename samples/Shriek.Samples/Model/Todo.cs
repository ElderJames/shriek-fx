using System;
using System.Collections.Generic;
using System.Text;

namespace Shriek.Samples.Model
{
    public class Todo
    {
        public Guid AggregateId { get; set; }

        public string Name { get; protected set; }

        public string Desception { get; protected set; }

        public DateTime FinishTime { get; protected set; }
    }
}
using System;

namespace Shriek.Samples.Models
{
    public class Todo
    {
        public Guid AggregateId { get; set; }

        public string Name { get; protected set; }

        public string Desception { get; protected set; }

        public DateTime FinishTime { get; protected set; }
    }
}
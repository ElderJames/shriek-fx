using System;

namespace Shriek.Samples.Models
{
    public class Todo
    {
        public Guid AggregateId { get; set; }

        public string Name { get; set; }

        public string Desception { get; set; }

        public DateTime FinishTime { get; set; }
    }
}
using System;
using System.ComponentModel.DataAnnotations;

namespace Shriek.Storage.Mementos
{
    public class Memento
    {
        [Key]
        public int Id { get; set; }

        public Guid aggregateId { get; set; }

        public int Version { get; set; }

        public string Data { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
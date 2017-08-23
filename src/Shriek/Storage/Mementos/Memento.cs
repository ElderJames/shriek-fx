using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace Shriek.Storage.Mementos
{
    public class Memento
    {
        [Key]
        public Guid Id { get; set; }

        public int Version { get; set; }

        public string Data { get; set; }

        public DateTime Timestamp => DateTime.Now;
    }
}
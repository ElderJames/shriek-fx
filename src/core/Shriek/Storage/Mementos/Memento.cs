using System;
using System.Collections.Generic;
using System.Text;

namespace Shriek.Storage.Mementos
{
    public class Memento
    {
        public Guid Id { get; set; }
        public int Version { get; set; }

        public IDictionary<string, object> Mapper { get; set; }
    }
}
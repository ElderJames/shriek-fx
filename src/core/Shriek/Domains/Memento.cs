using System;
using System.Collections.Generic;
using System.Text;

namespace Shriek.Domains
{
    public class Memento<AggregateKey>
    {
        public AggregateKey Id { get; set; }
        public int Version { get; set; }
    }

    public class Memento : Memento<Guid>
    {
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace Shriek.Commands
{
    public class Command
    {
        public int Version { get; set; }

        public Guid AggregateId { get; set; }
    }

    public class Command<TAggregateId> : Command, IAggregateCommand<TAggregateId>
    {
        public new TAggregateId AggregateId { get; set; }

        public Command(TAggregateId id, int version = 0)
        {
            AggregateId = id;
            Version = version;
        }
    }
}
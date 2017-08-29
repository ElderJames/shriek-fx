using Shriek.Messages;
using System;

namespace Shriek.Commands
{
    public class Command : Message
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
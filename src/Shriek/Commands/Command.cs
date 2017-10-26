using System;
using Shriek.Messages;

namespace Shriek.Commands
{
    public class Command : Message
    {
        public int Version { get; protected set; }

        public Guid AggregateId { get; protected set; }
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
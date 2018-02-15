using Shriek.Messages;
using System;

namespace Shriek.Commands
{
    public class Command
    {

        public Guid AggregateId { get; protected set; }
    }

    public class Command<TAggregateId> : Message, IAggregateCommand<TAggregateId>
    {
        public Command(TAggregateId aggregateId)
        {
            this.AggregateId = aggregateId;
        }
        public TAggregateId AggregateId { get; protected set; }

        public int Version { get; set; }

    }
}
using Shriek.Messages;
using System;

namespace Shriek.Commands
{
    public class Command<TAggregateId> : Command, IAggregateCommand<TAggregateId>
        where TAggregateId : IEquatable<TAggregateId>
    {
        public Command(TAggregateId aggregateId)
        {
            this.AggregateId = aggregateId;
        }

        public TAggregateId AggregateId { get; protected set; }
    }

    public abstract class Command : Message, ICommand
    {
    }
}
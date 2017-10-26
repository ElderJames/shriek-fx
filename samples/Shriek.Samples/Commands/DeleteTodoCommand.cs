using System;
using Shriek.Commands;

namespace Shriek.Samples.Commands
{
    public class DeleteTodoCommand : Command<Guid>
    {
        public DeleteTodoCommand(Guid aggregateId) : base(aggregateId)
        {
        }
    }
}
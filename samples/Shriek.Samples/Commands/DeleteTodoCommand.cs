using Shriek.Commands;
using System;

namespace Shriek.Samples.Commands
{
    public class DeleteTodoCommand : Command<Guid>
    {
        public DeleteTodoCommand(Guid aggregateId) : base(aggregateId)
        {
        }
    }
}
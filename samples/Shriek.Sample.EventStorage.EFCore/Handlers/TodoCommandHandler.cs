using Shriek.Storage;
using Shriek.Samples.Aggregates;
using Shriek.Samples.Commands;
using Shriek.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shriek.Sample.EventStorage.EFCore.Handlers
{
    public class TodoCommandHandler : ICommandHandler<CreateTodoCommand>,
        ICommandHandler<ChangeTodoCommand>
    {
        public TodoCommandHandler()
        {
        }

        public void Execute(ICommandContext context, CreateTodoCommand command)
        {
            var root = context.GetAggregateRoot(command.AggregateId, () => TodoAggregateRoot.Register(command));
        }

        public void Execute(ICommandContext context, ChangeTodoCommand command)
        {
            var root = context.GetAggregateRoot<TodoAggregateRoot>(command.AggregateId);
            if (root == null) return;
            root.Change(command);
        }
    }
}
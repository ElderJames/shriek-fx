using Shriek.Commands;
using Shriek.Samples.Aggregates;
using Shriek.Samples.Commands;
using System;

namespace Shriek.Samples.EventStorage.NoSql.Handlers
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
            var root = context.GetAggregateRoot<Guid, TodoAggregateRoot>(command.AggregateId);
            if (root == null) return;
            root.Change(command);
        }
    }
}
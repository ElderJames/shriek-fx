using Shriek.Domains;
using Shriek.Events;
using Shriek.Exceptions;
using Shriek.Samples.Commands;
using Shriek.Samples.Events;
using System;

namespace Shriek.Samples.Aggregates
{
    public class TodoAggregateRoot : AggregateRoot<Guid>, IHandle<TodoCreatedEvent>, IHandle<TodoChangedEvent>
    {
        public TodoAggregateRoot() : this(Guid.Empty)
        {
        }

        public TodoAggregateRoot(Guid aggregateId) : base(aggregateId)
        {
        }

        public string Name { get; protected set; }

        public string Desception { get; protected set; }

        public DateTime FinishTime { get; protected set; }

        public static TodoAggregateRoot Register(CreateTodoCommand command)
        {
            var root = new TodoAggregateRoot(command.AggregateId);
            root.Create(command);
            return root;
        }

        public void Create(CreateTodoCommand command)
        {
            if (command.FinishTime < DateTime.Now)
                throw new DomainException("结束时间不能早于当前时间");

            ApplyChange(new TodoCreatedEvent()
            {
                AggregateId = this.AggregateId,
                Name = command.Name,
                Desception = command.Desception,
                FinishTime = command.FinishTime
            });
        }

        public void Handle(TodoCreatedEvent e)
        {
            this.AggregateId = e.AggregateId;
            this.Name = e.Name;
            this.Desception = e.Desception;
            this.FinishTime = e.FinishTime;
        }

        public void Change(ChangeTodoCommand command)
        {
            if (command.FinishTime < DateTime.Now)
                throw new DomainException("结束时间不能早于当前时间");

            ApplyChange(new TodoChangedEvent()
            {
                AggregateId = this.AggregateId,
                Name = command.Name,
                Desception = command.Desception,
                FinishTime = command.FinishTime
            });
        }

        public void Handle(TodoChangedEvent e)
        {
            this.AggregateId = e.AggregateId;
            this.Name = e.Name;
            this.Desception = e.Desception;
            this.FinishTime = e.FinishTime;
        }
    }
}
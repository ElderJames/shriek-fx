using Shriek.Exceptions;
using Shriek.Events;
using Shriek.Samples.Events;
using Shriek.Samples.Commands;
using System;
using Shriek.Domains;

namespace Shriek.Samples.Aggregates
{
    public class TodoAggregateRoot : AggregateRoot, IHandle<TodoCreatedEvent>, IHandle<TodoChangedEvent>
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
                Version = this.Version,
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
            this.Version = e.Version;
            this.FinishTime = e.FinishTime;
        }

        public void Change(ChangeTodoCommand command)
        {
            if (command.FinishTime < DateTime.Now)
                throw new DomainException("结束时间不能早于当前时间");

            ApplyChange(new TodoChangedEvent()
            {
                AggregateId = this.AggregateId,
                Version = this.Version,
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
            this.Version = e.Version;
            this.FinishTime = e.FinishTime;
        }
    }
}
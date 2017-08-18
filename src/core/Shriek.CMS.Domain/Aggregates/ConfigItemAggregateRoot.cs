using Shriek.Events;
using Shriek.ConfigCenter.Domain.Events;
using Shriek.ConfigCenter.Domain.Commands;
using System;
using Shriek.Domains;
using Shriek.Storage.Mementos;

namespace Shriek.ConfigCenter.Domain.Aggregates
{
    public class ConfigItemAggregateRoot : AggregateRoot, IHandle<ConfigItemCreatedEvent>, IHandle<ConfigItemChangedEvent>
    {
        public ConfigItemAggregateRoot() : this(Guid.Empty)
        {
        }

        public ConfigItemAggregateRoot(Guid aggregateId) : base(aggregateId)
        {
        }

        public string Name { get; protected set; }

        public string Value { get; protected set; }

        public static ConfigItemAggregateRoot Register(CreateConfigItemCommand command)
        {
            var root = new ConfigItemAggregateRoot(command.AggregateId);
            root.Create(command);
            return root;
        }

        public void Create(CreateConfigItemCommand command)
        {
            ApplyChange(new ConfigItemCreatedEvent()
            {
                AggregateId = this.AggregateId,
                Version = this.Version,
                Name = command.Name,
                Value = command.Value
            });
        }

        public void Handle(ConfigItemCreatedEvent e)
        {
            this.AggregateId = e.AggregateId;
            this.Name = e.Name;
            this.Value = e.Value;
            this.Version = e.Version;
        }

        public void Change(ChangeConfigItemCommand command)
        {
            ApplyChange(new ConfigItemChangedEvent()
            {
                AggregateId = this.AggregateId,
                Version = this.Version,
                Name = command.Name,
                Value = command.Value
            });
        }

        public void Handle(ConfigItemChangedEvent e)
        {
            this.AggregateId = e.AggregateId;
            this.Name = e.Name;
            this.Value = e.Value;
            this.Version = e.Version;
        }
    }
}
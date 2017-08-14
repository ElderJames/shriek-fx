using System;
using Shriek.Domains;
using Shriek.Storage.Mementos;

namespace Shriek.CMS.Domain
{
    public class ConfigItemAggregateRoot : AggregateRoot<Guid>
    {
        public ConfigItemAggregateRoot(Guid aggregateId) : base(aggregateId)
        {
        }

        public string Name { get; protected set; }

        public string Value { get; protected set; }
    }
}
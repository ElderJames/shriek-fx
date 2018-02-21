using Shriek.Domains;
using Shriek.Events;
using Shriek.EventSourcing;
using Shriek.Storage.Mementos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shriek.Storage
{
    public class InMemoryEventStorage : AbstractEventStorage, IEventStorage
    {
        private readonly List<Event> events;
        private readonly IMementoRepository mementoRepository;
        
        public InMemoryEventStorage(IMementoRepository mementoRepository)
        {
            this.mementoRepository = mementoRepository;
            events = new List<Event>();
        }

        protected override IMementoRepository MementoRepository => mementoRepository;

        public override IEnumerable<Event> GetEvents<TKey>(TKey aggregateId, int afterVersion = 0)
        {
            var list = this.events.Select(x => x as Event<TKey>).Where(x => x.AggregateId.Equals(aggregateId));

            var events = list.Where(e => e.AggregateId.Equals(aggregateId) && e.Version >= afterVersion);

            return events;
        }

        public override IEvent<TKey> GetLastEvent<TKey>(TKey aggregateId)
        {
            var list = events.Select(x => x as IEvent<TKey>);

            return list.Where(e => e.AggregateId.Equals(aggregateId))
                .OrderBy(e => e.Version).LastOrDefault();
        }
        
        public override void Save(Event @event)
        {
            events.Add(@event);
        }
    }
}
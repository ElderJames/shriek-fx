using Shriek.Domains;
using Shriek.Events;
using Shriek.Storage.Mementos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shriek.Storage
{
    public class InMemoryEventStorage : IEventStorage, IEventOriginator
    {
        private readonly List<Event> eventCache;
        private readonly List<Memento> mementoCache;

        public InMemoryEventStorage()
        {
            eventCache = new List<Event>();
            mementoCache = new List<Memento>();
        }

        public IEnumerable<Event> GetEvents(Guid aggregateId, int afterVersion = 0)
        {
            var events = this.eventCache.Where(e => e.AggregateId == aggregateId && e.Version >= afterVersion);

            return events;
        }

        public Event GetLastEvent(Guid aggregateId)
        {
            return eventCache.Where(e => e.AggregateId == aggregateId)
                .OrderBy(e => e.Version).LastOrDefault();
        }

        public void SaveAggregateRoot<TAggregateRoot>(TAggregateRoot aggregate) where TAggregateRoot : IEventProvider, IAggregateRoot
        {
            var uncommittedChanges = aggregate.GetUncommittedChanges();
            var version = aggregate.Version;

            foreach (var @event in uncommittedChanges)
            {
                version++;
                if (version > 2)
                {
                    if (version % 3 == 0)
                    {
                        var originator = (IOriginator)aggregate;
                        var memento = originator.GetMemento();
                        memento.Version = version;
                        SaveMemento(memento);
                    }
                }
                @event.Version = version;
                Save(@event);
            }
        }

        public Memento GetMemento(Guid aggregateId)
        {
            var memento = mementoCache.Where(m => m.AggregateId == aggregateId).OrderBy(m => m.Version).LastOrDefault();
            return memento;
        }

        public void SaveMemento(Memento memento)
        {
            mementoCache.Add(memento);
        }

        public void Save<T>(T @event) where T : Event
        {
            eventCache.Add(@event);
        }

        public TAggregateRoot Source<TAggregateRoot>(Guid aggregateId) where TAggregateRoot : IAggregateRoot, IEventProvider, new()
        {
            //获取该记录的所有缓存事件
            IEnumerable<Event> events;
            Memento memento = null;
            var obj = new TAggregateRoot();

            if (obj is IOriginator)
            {
                //获取该记录的更改快照
                memento = GetMemento(aggregateId);
            }

            if (memento != null)
            {
                //获取该记录最后一次快照之后的更改，避免加载过多历史更改
                events = GetEvents(aggregateId, memento.Version);
                //从快照恢复
                ((IOriginator)obj).SetMemento(memento);
            }
            else
            {
                //获取所有历史更改记录
                events = GetEvents(aggregateId);
            }

            if (memento == null && !events.Any())
                return default(TAggregateRoot);

            //重现历史更改
            obj.LoadsFromHistory(events);
            return obj;
        }
    }
}
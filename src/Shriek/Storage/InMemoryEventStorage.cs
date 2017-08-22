using System;
using System.Collections.Generic;
using System.Linq;
using Shriek.Events;
using Shriek.Domains;
using Shriek.Storage.Mementos;

namespace Shriek.Storage
{
    public class InMemoryEventStorage : IEventStorage, IEventOriginator
    {
        private List<Event> _events;
        private List<Memento> _mementoes;

        public InMemoryEventStorage()
        {
            _events = new List<Event>();
            _mementoes = new List<Memento>();
        }

        public IEnumerable<Event> GetEvents(Guid aggregateId)
        {
            var events = _events.Where(p => p.AggregateId == aggregateId);
            if (events.Count() == 0)
            {
                return new Event[] { };
            }
            return events;
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

        public T GetMemento<T>(Guid aggregateId) where T : Memento
        {
            var memento = _mementoes.Where(m => m.Id == aggregateId).Select(m => m).LastOrDefault();
            if (memento != null)
            {
                return (T)memento;
            }
            return null;
        }

        public void SaveMemento(Memento memento)
        {
            _mementoes.Add(memento);
        }

        public void Save<T>(T @event) where T : Event
        {
            _events.Add(@event);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Shriek.Events;
using Shriek.Domains;
using Shriek.Storage.Mementos;
using Shriek.Utils;

namespace Shriek.Storage
{
    public class InMemoryEventStorage : IEventStorage
    {
        private List<Event> _events;
        private List<Memento> _mementoes;
        private readonly IEventBus _eventBus;

        public InMemoryEventStorage(IEventBus eventBus)
        {
            _events = new List<Event>();
            _mementoes = new List<Memento>();
            _eventBus = eventBus;
        }

        public IEnumerable<Event> GetEvents(Guid aggregateId)
        {
            var events = _events.Where(p => p.AggregateId == aggregateId);
            //if (events.Count() == 0)
            //{
            //    throw new Exception();
            //}
            return events;
        }

        public void Save(AggregateRoot aggregate)
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
                _events.Add(@event);
            }
            foreach (var @event in uncommittedChanges)
            {
                var desEvent = (dynamic)@event; /*(Event)@event.As(@event.GetType());*/
                _eventBus.Publish(desEvent);
            }
        }

        public T GetMemento<T>(Guid aggregateId) where T : Memento
        {
            var memento = _mementoes.Where(m => m.Id.Equals(aggregateId)).Select(m => m).LastOrDefault();
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

        //public void Save<T>(T theEvent) where T : Event
        //{
        //    _events.Add(theEvent);
        //}

        //public T GetMemento<T>(TKey aggregateId) where T : Memento
        //{
        //    //var memento = _mementoes.Where(m => m.Id == aggregateId).Select(m => m).LastOrDefault();
        //    //if (memento != null)
        //    //{
        //    //    return (T)memento;
        //    //}
        //    return default(T);
        //}
    }
}
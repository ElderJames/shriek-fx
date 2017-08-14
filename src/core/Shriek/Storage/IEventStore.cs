using Shriek.Domains;
using Shriek.Events;
using System;
using System.Collections.Generic;
using System.Text;
using Shriek.Storage.Mementos;

namespace Shriek.Storage
{
    public interface IEventStore
    {
        IEnumerable<Event> GetEvents(Guid aggregateId);

        void Save<T>(T theEvent) where T : Event;

        T GetMemento<T, TKey>(TKey aggregateId) where T : Memento;

        void SaveMemento(Memento memento);
    }
}
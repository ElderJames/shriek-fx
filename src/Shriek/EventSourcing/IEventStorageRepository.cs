using Shriek.Events;
using Shriek.Storage;
using System;
using System.Collections.Generic;

namespace Shriek.EventSourcing
{
    public interface IEventStorageRepository : IDisposable
    {
        void Store(StoredEvent theEvent);

        Event GetLastEvent<TKey>(TKey aggregateId) where TKey : IEquatable<TKey>;

        IEnumerable<StoredEvent> GetEvents<TKey>(TKey aggregateId, int afterVersion = 0) where TKey : IEquatable<TKey>;
    }
}
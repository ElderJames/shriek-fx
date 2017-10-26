using System;
using System.Collections.Generic;
using Shriek.Storage;

namespace Shriek.EventSourcing
{
    public interface IEventStorageRepository : IDisposable
    {
        void Store(StoredEvent theEvent);

        StoredEvent GetLastEvent<TKey>(TKey aggregateId) where TKey : IEquatable<TKey>;

        IEnumerable<StoredEvent> GetEvents<TKey>(TKey aggregateId, int afterVersion = 0) where TKey : IEquatable<TKey>;
    }
}
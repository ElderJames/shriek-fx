using Shriek.Storage;
using System;
using System.Collections.Generic;

namespace Shriek.EventSourcing
{
    public interface IEventStorageRepository : IDisposable
    {
        void Store(StoredEvent theEvent);

        StoredEvent GetLastEvent<TKey>(TKey eventId) where TKey : IEquatable<TKey>;

        IEnumerable<StoredEvent> GetEvents<TKey>(TKey eventId, int afterVersion = 0) where TKey : IEquatable<TKey>;
    }
}
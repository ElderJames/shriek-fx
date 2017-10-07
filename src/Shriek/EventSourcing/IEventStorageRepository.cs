using Shriek.Storage;
using System;
using System.Collections.Generic;

namespace Shriek.EventSourcing
{
    public interface IEventStorageRepository : IDisposable
    {
        void Store(StoredEvent theEvent);

        StoredEvent GetLastEvent(Guid aggregateId);

        IEnumerable<StoredEvent> GetEvents(Guid aggregateId, int afterVersion = 0);
    }
}
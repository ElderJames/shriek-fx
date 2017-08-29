using Shriek.Events;
using Shriek.Storage;
using System;
using System.Collections.Generic;

namespace Shriek.EventSourcing
{
    public interface IEventStorageRepository : IDisposable
    {
        void Store(StoredEvent theEvent);

        Event GetLastEvent(Guid aggregateId);

        IEnumerable<StoredEvent> All(Guid aggregateId);
    }
}
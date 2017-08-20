using Shriek.Events;
using System;
using System.Collections.Generic;

namespace Shriek.EventSourcing
{
    public interface IEventStorageRepository : IDisposable
    {
        void Store(StoredEvent theEvent);

        IEnumerable<StoredEvent> All(Guid aggregateId);
    }
}
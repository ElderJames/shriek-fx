using System;
using System.Collections.Generic;
using System.Text;
using Shriek.Events;

namespace Shriek.EventSourcing
{
    public class EventStorageSQLRepository : IEventStorageRepository
    {
        public IList<StoredEvent> All(Guid aggregateId)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Store(StoredEvent theEvent)
        {
            throw new NotImplementedException();
        }
    }
}
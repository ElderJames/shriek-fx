using System;
using System.Collections.Generic;
using System.Text;
using Shriek.Events;
using System.Linq;
using Shriek.EventSourcing;
using Shriek.Storage;

namespace Shriek.EventStorage.EFCore
{
    public class EventStorageSQLRepository : IEventStorageRepository
    {
        private EventStorageSQLContext context;

        public EventStorageSQLRepository(EventStorageSQLContext context)
        {
            this.context = context;
        }

        public IEnumerable<StoredEvent> All(Guid aggregateId)
        {
            return context.Set<StoredEvent>().Where(e => e.AggregateId == aggregateId).AsEnumerable();
        }

        public void Dispose()
        {
            context.Dispose();
        }

        public void Store(StoredEvent theEvent)
        {
            context.Set<StoredEvent>().Add(theEvent);
            context.SaveChanges();
        }
    }
}
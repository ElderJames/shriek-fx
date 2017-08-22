using Microsoft.EntityFrameworkCore;
using Shriek.EntityFrameworkCore;
using Shriek.Events;

namespace Shriek.EventStorage.EFCore
{
    public class EventStorageSQLContext : BaseDbContext
    {
        public EventStorageSQLContext(DbContextOptions<EventStorageSQLContext> options) : base(options)
        {
        }

        public DbSet<StoredEvent> StoredEvent { get; set; }
    }
}
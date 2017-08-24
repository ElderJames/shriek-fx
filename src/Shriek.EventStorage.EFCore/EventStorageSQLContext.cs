using Microsoft.EntityFrameworkCore;
using Shriek.EntityFrameworkCore;
using Shriek.Storage;

namespace Shriek.EventStorage.EFCore
{
    public class EventStorageSQLContext : BaseDbContext
    {
        public EventStorageSQLContext(DbContextOptions<EventStorageSQLContext> options) : base(options)
        {
        }
    }
}
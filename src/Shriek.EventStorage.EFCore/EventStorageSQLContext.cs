using Microsoft.EntityFrameworkCore;
using Shriek.EntityFrameworkCore;

namespace Shriek.EventStorage.EFCore
{
    public class EventStorageSQLContext : BaseDbContext
    {
        public EventStorageSQLContext(DbContextOptions<EventStorageSQLContext> options) : base(options)
        {
        }
    }
}
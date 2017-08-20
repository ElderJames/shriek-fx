using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Shriek.EntityFrameworkCore;
using Shriek.Events;
using Shriek.EventStorage.EF;

namespace Shriek.EventSourcing.Sql.EFCore
{
    public class EventStorageSQLContext : BaseDbContext
    {
        public EventStorageSQLContext(DbContextOptions<EventStorageSQLContext> options) : base(options)
        {
        }

        public DbSet<StoredEvent> StoredEvent { get; set; }
    }
}
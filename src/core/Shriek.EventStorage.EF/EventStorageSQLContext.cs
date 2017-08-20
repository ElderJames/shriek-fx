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
        public EventStorageSQLContext()
        {
        }

        public DbSet<StoredEvent> StoredEvent { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new StoredEventMap());
            base.OnModelCreating(modelBuilder);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using Shriek.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shriek.EntityFrameworkCore;

namespace Shriek.EventStorage.EF
{
    internal class StoredEventMap : IEntityTypeConfiguration<StoredEvent>
    {
        public void Configure(EntityTypeBuilder<StoredEvent> builder)
        {
            builder.Property(c => c.Timestamp)
                .HasColumnName("CreationDate");

            builder.Property(c => c.EventType)
                .HasColumnName("Action")
                .HasColumnType("varchar(100)");
        }
    }
}